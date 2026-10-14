using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using DAVE.Models;

namespace DAVE.Cli;

/// <summary>
/// The dave-cli JSON report: one entry per check, in registration order, as the results grid and query email show it
/// </summary>
public static class CheckReport
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Indented = true,
        NewLine = "\n",
        // the report is read by tools, not embedded in HTML, so keep query text readable (quotes, £, etc.)
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Writes the report as UTF-8 JSON (no byte order mark), followed by a newline
    /// </summary>
    public static byte[] ToJson(string daveVersion, string gitSha, string file, string? previousFile, IEnumerable<CheckBase> checks)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, WriterOptions))
        {
            writer.WriteStartObject();
            writer.WriteString("tool", "dave-cli");
            writer.WriteString("dave_version", daveVersion);
            writer.WriteString("git_sha", gitSha);
            writer.WriteString("file", file);
            if (previousFile != null)
                writer.WriteString("previous_file", previousFile);
            else
                writer.WriteNull("previous_file");

            writer.WriteStartArray("checks");
            var index = 0;
            foreach (var check in checks)
            {
                // most checks work Pass out on each read, so read it once
                var passed = check.Pass;

                writer.WriteStartObject();
                writer.WriteNumber("index", index++);
                writer.WriteString("number", check.Number.ToString(CultureInfo.InvariantCulture));
                writer.WriteString("kind", KindOf(check));
                writer.WriteString("name", check.Name);
                writer.WriteBoolean("passed", passed);
                // the email lists the query message of each failed check, verbatim
                writer.WriteString("message", passed ? string.Empty : check.QueryMessage);
                writer.WriteString("current", check.Current ?? string.Empty);
                // the grid's "Previous" column (CheckBase.Previous); the report contract names it "expected"
                writer.WriteString("expected", check.Previous ?? string.Empty);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        stream.WriteByte((byte)'\n');
        return stream.ToArray();
    }

    /// <summary>
    /// The check's class name, without any generic arity (e.g. CheckGreaterOrEqual)
    /// </summary>
    public static string KindOf(CheckBase check)
    {
        var name = check.GetType().Name;
        var arity = name.IndexOf('`');
        return arity < 0 ? name : name[..arity];
    }
}
