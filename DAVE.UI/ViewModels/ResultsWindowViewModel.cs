using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DAVE.Models;

namespace DAVE.ViewModels;

public partial class ResultsWindowViewModel() : ViewModelBase
{
    private const string EmailSubject = "[ACTION REQUIRED] - WRAP: FWRR Submission Queries";

    private const string EmailBodyIntro = @"
Thank you for your submission.

We have done some preliminary automated checks which have raised the following queries, which we kindly ask you to review and reply with any updates or explanations.
";

    private const string EmailBodyOutro = @"

Thank you in advance for your time and responses. 

";


    private DataCaptureSpreadsheet? CurrentSheet { get; }
    private DataCaptureSpreadsheet? PreviousSheet { get; }

    public ObservableCollection<CheckBase> Results { get; set; } = [];

    public bool IsEmailEnabled => Results.Any(r => !r.Pass);
    public void OnEmailClicked()
    {
        try
        {
            var body = EmailBodyIntro;

            foreach (var queryMessage in Results
                         .Where(r => !r.Pass)
                         .Select(r => r.QueryMessage))
            {
                body += $"{Environment.NewLine}\t• {queryMessage}";
            }

            body += EmailBodyOutro;

            var mailto = $"mailto:?subject={Uri.EscapeDataString(EmailSubject)}&body={Uri.EscapeDataString(body)}";

            Process.Start(new ProcessStartInfo(mailto) { UseShellExecute = true });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ResultsWindowViewModel(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet) : this()
    {
        CurrentSheet = currentSheet;
        PreviousSheet = previousSheet;
    }

    public string ResultsStats
    {
        get
        {
            return Results.Any()
                ? $"{Results.Count(r => r.Pass)} / {Results.Count}  ({(Results.Count(r => r.Pass) / (decimal)Results.Count):P0})"
                : "- / -  (-%)";
        }
    }

    public void Verify()
    {
        if (CurrentSheet is null)
            return;

        foreach (var check in CheckSet.All(CurrentSheet, PreviousSheet))
            Results.Add(check);

        OnPropertyChanged(nameof(IsEmailEnabled));
        OnPropertyChanged(nameof(ResultsStats));
    }
}
