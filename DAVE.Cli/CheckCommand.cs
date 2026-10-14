using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DAVE.Models;

namespace DAVE.Cli;

/// <summary>
/// dave-cli check &lt;current.xlsx&gt; [--previous &lt;previous.xlsx&gt;] --json
/// </summary>
public static class CheckCommand
{
    public const int ExitRan = 0;
    public const int ExitError = 1;
    public const int ExitNotADataCaptureSheet = 2;

    private const string Usage = "usage: dave-cli check <current.xlsx> [--previous <previous.xlsx>] --json";

    public static string DaveVersion =>
        typeof(CheckCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

    public static string GitSha =>
        typeof(CheckCommand).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "GitSha")?.Value is { Length: > 0 } sha
            ? sha
            : "unknown";

    /// <summary>
    /// Runs the checks and writes the JSON report to stdout; diagnostics go to stderr
    /// </summary>
    /// <returns>0 when the checks ran (whatever the verdicts), 2 when a file is missing or not a Data Capture Sheet, otherwise 1</returns>
    public static int Run(string[] args, Stream stdout, TextWriter stderr)
    {
        try
        {
            if (args is ["--help"] or ["-h"])
            {
                stderr.WriteLine(Usage);
                return ExitRan;
            }

            if (!TryParseArguments(args, out var currentPath, out var previousPath, out var error))
            {
                stderr.WriteLine($"error: {error}");
                stderr.WriteLine(Usage);
                return ExitError;
            }

            var currentFile = Path.GetFullPath(currentPath);
            var previousFile = previousPath != null ? Path.GetFullPath(previousPath) : null;

            var current = Load(currentFile, stderr);
            if (current == null)
                return ExitNotADataCaptureSheet;

            DataCaptureSpreadsheet? previous = null;
            if (previousFile != null)
            {
                previous = Load(previousFile, stderr);
                if (previous == null)
                    return ExitNotADataCaptureSheet;
            }

            // The desktop app picks the current/previous slot from the submission date; here the caller picks, so only warn
            if (!current.IsCurrent)
                stderr.WriteLine($"warning: {currentFile}: submission date is not in the current year, the desktop app would load it as a previous submission");
            if (previous is { IsPrevious: false })
                stderr.WriteLine($"warning: {previousFile}: submission date is not before the current year, the desktop app would load it as a current submission");

            // Build the whole report before writing any of it, so a failure leaves stdout empty
            var report = CheckReport.ToJson(DaveVersion, GitSha, currentFile, previousFile, CheckSet.All(current, previous));
            stdout.Write(report);
            stdout.Flush();
            return ExitRan;
        }
        catch (Exception ex)
        {
            stderr.WriteLine($"error: the checks could not be run: {ex.Message}");
            return ExitError;
        }
    }

    private static bool TryParseArguments(string[] args, out string currentPath, out string? previousPath, out string error)
    {
        currentPath = string.Empty;
        previousPath = null;
        error = string.Empty;

        if (args.Length == 0 || args[0] != "check")
        {
            error = "expected the 'check' command";
            return false;
        }

        string? current = null;
        for (var i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--json":
                    // JSON is the only output format
                    break;
                case "--previous":
                    if (i + 1 >= args.Length || previousPath != null)
                    {
                        error = "--previous takes one file";
                        return false;
                    }
                    previousPath = args[++i];
                    break;
                default:
                    if (args[i].StartsWith("--") || current != null)
                    {
                        error = $"unexpected argument '{args[i]}'";
                        return false;
                    }
                    current = args[i];
                    break;
            }
        }

        if (current == null)
        {
            error = "no current submission file given";
            return false;
        }

        currentPath = current;
        return true;
    }

    private static DataCaptureSpreadsheet? Load(string file, TextWriter stderr)
    {
        if (!File.Exists(file))
        {
            stderr.WriteLine($"error: {file}: file not found");
            return null;
        }

        var sheet = new DataCaptureSpreadsheet(file);
        if (sheet.IsValid)
            return sheet;

        stderr.WriteLine($"error: {file}: could not be opened as a Data Capture Sheet");
        return null;
    }
}
