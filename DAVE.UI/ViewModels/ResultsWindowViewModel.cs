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

    public ObservableCollection<CheckBase> Results { get; set; } = [];

    public bool IsEmailEnabled => Results.Any(r => !r.Pass);
    public void OnEmailPressed()
    {
        try
        {
            var body = string.Join(
                Environment.NewLine,
                Results
                    .Where(r => !r.Pass)
                    .Select(r => r.QueryMessage));

            var mailto = "mailto:?body=" + Uri.EscapeDataString(body);

            Process.Start(new ProcessStartInfo(mailto) { UseShellExecute = true });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void OnClosePressed()
    {

    }


    public ResultsWindowViewModel() : this(new VerifyService()) { }

    public ResultsWindowViewModel(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet) : this()
    {
        CurrentSheet = currentSheet;
        PreviousSheet = previousSheet;
    }

    public void Verify()
    {
        Results.Add(new CheckIfGiven(1, "Company Name",     CurrentSheet.GetValue<string>(DataFieldName.CompanyName), PreviousSheet?.GetValue<string>(DataFieldName.CompanyName), "Row 8: Incomplete response. Please confirm the Company Name to which the data capture sheet refers"));
        Results.Add(new CheckIfGiven(2, "Annual Turnover",  CurrentSheet.GetValue<string>(DataFieldName.AnnualTurnover), PreviousSheet?.GetValue<string>(DataFieldName.AnnualTurnover), "Row 13: New requirement of updated form - Annual turnover missing. Please could you provide? This is required purely for categorising the size of the business and is not shared."));
        Results.Add(new CheckNumberComparison(3, "Annual Turnover Comparable?", CurrentSheet.GetValue<decimal>(DataFieldName.AnnualTurnover), PreviousSheet?.GetValue<decimal>(DataFieldName.AnnualTurnover), 20, "Row 13: There appears to be a significant change in your turnover. Please clarify why this is the case."));
        Results.Add(new CheckDateRange(4, "Inventory 12 months", CurrentSheet.GetValue<DateTime>(DataFieldName.InventoryPeriodStart), CurrentSheet.GetValue<DateTime>(DataFieldName.InventoryPeriodEnd), 5, "Rows 17/18: Inventory period must cover 12 months (one year). Please resubmit your data for a 12 month period or advise why it is not possible to do so."));
        Results.Add(new CheckDateRangeContinuous(5, "Inventory Continuous",
            CurrentSheet.GetValue<DateTime>(DataFieldName.InventoryPeriodStart),
            CurrentSheet.GetValue<DateTime>(DataFieldName.InventoryPeriodEnd),
            PreviousSheet?.GetValue<DateTime>(DataFieldName.InventoryPeriodStart),
            PreviousSheet?.GetValue<DateTime>(DataFieldName.InventoryPeriodEnd), 5,
            "Rows 17/18: The 12 month period should be continuous from your previous submission. Please ensure there is no gap in reporting or double counting by having an overlap. Please resubmit your data such that it is continuous from your last submission or advise why it is not possible to do so?"));


        OnPropertyChanged(nameof(IsEmailEnabled));
    }
}
