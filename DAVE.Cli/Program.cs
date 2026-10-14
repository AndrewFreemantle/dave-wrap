using System;
using System.Globalization;

namespace DAVE.Cli;

sealed class Program
{
    public static int Main(string[] args)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        // Submissions are read as en-GB, and the grid's dates and currency are shown as on a UK desktop.
        //  Pinning it here keeps the report the same whatever machine the CLI runs on
        var gbCulture = CultureInfo.GetCultureInfo("en-GB");
        CultureInfo.DefaultThreadCurrentCulture = gbCulture;
        CultureInfo.CurrentCulture = gbCulture;

        // stdout carries only the JSON report; anything else written to the console goes to stderr
        var stdout = Console.OpenStandardOutput();
        Console.SetOut(Console.Error);

        return CheckCommand.Run(args, stdout, Console.Error);
    }
}
