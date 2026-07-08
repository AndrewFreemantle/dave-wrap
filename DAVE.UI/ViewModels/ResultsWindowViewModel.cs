using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DAVE.Models;
using DAVE.Services;

namespace DAVE.ViewModels;

public partial class ResultsWindowViewModel(IVerifyService verifyService) : ViewModelBase
{
    private DataCaptureSpreadsheet CurrentSheet { get; }
    private DataCaptureSpreadsheet? PreviousSheet { get; }

    public ObservableCollection<Check> Results { get; set; } = [];

    public bool IsEmailEnabled => Results.Any(r => !r.Pass);
    public void OnEmailPressed()
    {
        var body = string.Join(
            Environment.NewLine,
            Results
                .Select(r => r.QueryMessage));

        var mailto = "mailto:?body=" + Uri.EscapeDataString(body);

        Process.Start(new ProcessStartInfo(mailto) { UseShellExecute = true });
    }


    public ResultsWindowViewModel() : this(new VerifyService()) { }

    public ResultsWindowViewModel(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet) : this()
    {
        CurrentSheet = currentSheet;
        PreviousSheet = previousSheet;
    }

    public void Verify()
    {
        // 8	Company Name - is it included?	IF blank, please raise query.		Row 8: Incomplete response. Please confirm the Company Name to which the data capture sheet refers
        var current = CurrentSheet.GetValue(DataFieldName.CompanyName).ToString();
        var previous = PreviousSheet?.GetValue(DataFieldName.CompanyName).ToString();

        var item = new Check
        {
            Number = 1,
            Name = "Company Name - is it included?",
            Current = current,
            Previous = previous,
            Pass = !(string.IsNullOrWhiteSpace(current)),
            QueryMessage = "Row 8: Incomplete response. Please confirm the Company Name to which the data capture sheet refers"
        };

        Results.Add(item);
        OnPropertyChanged(nameof(IsEmailEnabled));
    }


}

public class Check
{
    public int Number { get; set; }
    public required string Name { get; set; }
    public string? Current { get; set; }
    public string? Previous { get; set; }
    public bool Pass { get; set; }
    public required string QueryMessage { get; set; }
}
