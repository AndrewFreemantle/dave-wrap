using System.Text;
using System.Text.Json;
using DAVE.Cli;
using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckReportTests
{
    private const string File = "/returns/current.xlsx";
    private const string PreviousFile = "/returns/previous.xlsx";

    private static readonly DataCaptureSpreadsheet CurrentSheet = new(DataCaptureSheetBuilder.Current().Build());
    private static readonly DataCaptureSpreadsheet PreviousSheet = new(DataCaptureSheetBuilder.Previous().Build());

    private static JsonElement Report(DataCaptureSpreadsheet? previous = null, string? previousFile = null) =>
        JsonDocument.Parse(CheckReport.ToJson("1.0.7", "abc1234", File, previousFile, CheckSet.All(CurrentSheet, previous))).RootElement;

    [Fact]
    public void Report_TopLevelProperties_InContractOrder()
    {
        var report = Report();

        Assert.Equal(["tool", "dave_version", "git_sha", "file", "previous_file", "checks"],
            report.EnumerateObject().Select(p => p.Name));
        Assert.Equal("dave-cli", report.GetProperty("tool").GetString());
        Assert.Equal("1.0.7", report.GetProperty("dave_version").GetString());
        Assert.Equal("abc1234", report.GetProperty("git_sha").GetString());
        Assert.Equal(File, report.GetProperty("file").GetString());
        Assert.Equal(JsonValueKind.Null, report.GetProperty("previous_file").ValueKind);
    }

    [Fact]
    public void Report_CheckProperties_InContractOrder()
    {
        foreach (var check in Report().GetProperty("checks").EnumerateArray())
        {
            Assert.Equal(["index", "number", "kind", "name", "passed", "message", "current", "expected"],
                check.EnumerateObject().Select(p => p.Name));
        }
    }

    [Fact]
    public void Report_ListsEveryCheck_InRegistrationOrder()
    {
        var checks = CheckSet.All(CurrentSheet, PreviousSheet);
        var reported = Report(PreviousSheet, PreviousFile).GetProperty("checks").EnumerateArray().ToList();

        Assert.Equal(45, reported.Count);
        Assert.Equal(Enumerable.Range(0, 45), reported.Select(c => c.GetProperty("index").GetInt32()));
        Assert.Equal(checks.Select(c => c.Number.ToString()), reported.Select(c => c.GetProperty("number").GetString()));
        Assert.Equal(checks.Select(c => c.Name), reported.Select(c => c.GetProperty("name").GetString()));
        Assert.Equal(checks.Select(c => c.Pass), reported.Select(c => c.GetProperty("passed").GetBoolean()));
    }

    [Fact]
    public void Report_GridColumns_AsTheGridShowsThem()
    {
        var checks = CheckSet.All(CurrentSheet, PreviousSheet);
        var reported = Report(PreviousSheet, PreviousFile).GetProperty("checks").EnumerateArray().ToList();

        Assert.Equal(checks.Select(c => c.Current), reported.Select(c => c.GetProperty("current").GetString()));
        Assert.Equal(checks.Select(c => c.Previous ?? string.Empty), reported.Select(c => c.GetProperty("expected").GetString()));
    }

    [Fact]
    public void Report_DuplicateAndMissingNumbers_ArePreserved()
    {
        var numbers = Report().GetProperty("checks").EnumerateArray()
            .Select(c => c.GetProperty("number").GetString())
            .ToList();

        Assert.Equal(2, numbers.Count(n => n == "13"));
        Assert.Equal(2, numbers.Count(n => n == "24"));
        Assert.DoesNotContain("22", numbers);
    }

    [Fact]
    public void Report_FailedCheck_CarriesTheEmailQueryVerbatim()
    {
        var checks = CheckSet.All(CurrentSheet, null);
        var reported = Report().GetProperty("checks").EnumerateArray().ToList();

        // Country was not given, so check 6 "United Kingdom" fails
        Assert.False(reported[5].GetProperty("passed").GetBoolean());
        Assert.Equal(checks[5].QueryMessage, reported[5].GetProperty("message").GetString());
    }

    [Fact]
    public void Report_PassedCheck_HasAnEmptyMessage()
    {
        var reported = Report().GetProperty("checks").EnumerateArray().ToList();

        // Company Name was given, so check 1 passes
        Assert.True(reported[0].GetProperty("passed").GetBoolean());
        Assert.Equal("Acme Foods", reported[0].GetProperty("current").GetString());
        Assert.Equal(string.Empty, reported[0].GetProperty("message").GetString());
    }

    [Fact]
    public void Report_Kind_IsTheCheckClassName()
    {
        var reported = Report().GetProperty("checks").EnumerateArray().ToList();

        Assert.Equal("CheckIfGiven", reported[0].GetProperty("kind").GetString());
        Assert.Equal("CheckGreaterOrEqual", reported[11].GetProperty("kind").GetString());
        Assert.Equal("CheckGreaterOrEqual", reported[24].GetProperty("kind").GetString());
    }

    [Fact]
    public void Report_WithPrevious_NamesThePreviousFile()
    {
        Assert.Equal(PreviousFile, Report(PreviousSheet, PreviousFile).GetProperty("previous_file").GetString());
    }

    [Fact]
    public void Report_IsUtf8_WithoutByteOrderMark_AndUnescapedQueryText()
    {
        var bytes = CheckReport.ToJson("1.0.7", "abc1234", File, null, CheckSet.All(CurrentSheet, null));
        var text = Encoding.UTF8.GetString(bytes);

        Assert.NotEqual(0xEF, bytes[0]);
        Assert.Contains("Please complete Columns B with \\\"Yes\\\"", text);
        Assert.Contains("'United Kingdom'", text);
        Assert.EndsWith("}\n", text);
    }
}
