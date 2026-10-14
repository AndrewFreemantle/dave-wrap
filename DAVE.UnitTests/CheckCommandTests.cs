using DAVE.Cli;

namespace DAVE.UnitTests;

public class CheckCommandTests
{
    private static (int ExitCode, string Stdout, string Stderr) Run(params string[] args)
    {
        using var stdout = new MemoryStream();
        using var stderr = new StringWriter();
        var exitCode = CheckCommand.Run(args, stdout, stderr);
        return (exitCode, System.Text.Encoding.UTF8.GetString(stdout.ToArray()), stderr.ToString());
    }

    [Fact]
    public void Run_NoArguments_ExitsWithError()
    {
        var (exitCode, stdout, _) = Run();

        Assert.Equal(CheckCommand.ExitError, exitCode);
        Assert.Empty(stdout);
    }

    [Fact]
    public void Run_NoCurrentFile_ExitsWithError()
    {
        Assert.Equal(CheckCommand.ExitError, Run("check", "--json").ExitCode);
    }

    [Fact]
    public void Run_UnknownOption_ExitsWithError()
    {
        Assert.Equal(CheckCommand.ExitError, Run("check", "current.xlsx", "--yaml").ExitCode);
    }

    [Fact]
    public void Run_PreviousWithoutFile_ExitsWithError()
    {
        Assert.Equal(CheckCommand.ExitError, Run("check", "current.xlsx", "--previous").ExitCode);
    }

    [Fact]
    public void Run_MissingFile_ExitsAsNotADataCaptureSheet()
    {
        var missing = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");

        var (exitCode, stdout, stderr) = Run("check", missing, "--json");

        Assert.Equal(CheckCommand.ExitNotADataCaptureSheet, exitCode);
        Assert.Empty(stdout);
        Assert.Contains("file not found", stderr);
    }

    [Fact]
    public void Run_NotASpreadsheet_ExitsAsNotADataCaptureSheet()
    {
        var notASpreadsheet = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        File.WriteAllText(notASpreadsheet, "not a spreadsheet");
        try
        {
            var (exitCode, stdout, stderr) = Run("check", notASpreadsheet, "--json");

            Assert.Equal(CheckCommand.ExitNotADataCaptureSheet, exitCode);
            Assert.Empty(stdout);
            Assert.Contains("could not be opened as a Data Capture Sheet", stderr);
        }
        finally
        {
            File.Delete(notASpreadsheet);
        }
    }

    [Fact]
    public void Run_MissingPreviousFile_ExitsAsNotADataCaptureSheet()
    {
        var notASpreadsheet = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        File.WriteAllText(notASpreadsheet, "not a spreadsheet");
        try
        {
            var missing = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");

            Assert.Equal(CheckCommand.ExitNotADataCaptureSheet, Run("check", notASpreadsheet, "--previous", missing).ExitCode);
        }
        finally
        {
            File.Delete(notASpreadsheet);
        }
    }
}
