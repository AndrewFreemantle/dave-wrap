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
    public void OnEmailClicked()
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

    public ResultsWindowViewModel() : this(new VerifyService()) { }

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
        Results.Add(new CheckMatch(6, "United Kingdom",     CurrentSheet.GetValue<string>(DataFieldName.Country), PreviousSheet?.GetValue<string>(DataFieldName.Country), "United Kingdom", "Row 22: You have not selected 'United Kingdom' as the country in scope. Please provide more details/confirm this. If your data covers sites outside of the UK, for the purposes of the UK Food Waste Reduction Roadmap reporting, please submit a Data Capture sheet for UK sites only."));
        Results.Add(new CheckIfGiven(7, "Business Sector",  CurrentSheet.GetValue<string>(DataFieldName.Sector), PreviousSheet?.GetValue<string>(DataFieldName.Sector), "Row 23: Incomplete response. Please advise on the business sector that you feel best fits your business from the drop-down list provided."));
        Results.Add(new CheckIfGiven(8, "Lifecycle",  CurrentSheet.GetValue<string>(DataFieldName.Lifecycle), PreviousSheet?.GetValue<string>(DataFieldName.Lifecycle), "Row 24: Incomplete response. Please advise on the lifecycle stages under your control covered by your reporting e.g. direct operations (manufacturing, warehouses)."));
        Results.Add(new CheckIfGiven(9, "Sites Total",  CurrentSheet.GetValue<string>(DataFieldName.SitesTotal), PreviousSheet?.GetValue<string>(DataFieldName.SitesTotal), "Row 26: Incomplete response. Please provide the total number of sites operated by your business in the geographical area of this report e.g. UK."));
        Results.Add(new CheckIfGiven(10, "Sites Covered",  CurrentSheet.GetValue<string>(DataFieldName.SitesTotal), PreviousSheet?.GetValue<string>(DataFieldName.SitesTotal), "Row 27: Incomplete response. Please provide the number of sites covered by this report. This figure may differ from total number of sites if some sites have been excluded from reporting e.g. due to minimal food handling or food waste (e.g offices), out of reporting scope (e.g farms), outside organisational boundary (e.g franchise sites without operational control), not operational during reporting period (e.g. under construction, permanently closed)."));
        Results.Add(new CheckIfGiven(11, "Sites Contributing",  CurrentSheet.GetValue<string>(DataFieldName.SitesCovered), PreviousSheet?.GetValue<string>(DataFieldName.SitesCovered), "Row 28: Incomplete response. Please provide the number of sites directly contributing data. This may differ from sites covered by report where sites are within the reporting boundary but do not contribute data due to missing data, incomplete measurement systems, data gaps, or insufficient data quality."));


        OnPropertyChanged(nameof(IsEmailEnabled));
        OnPropertyChanged(nameof(ResultsStats));
    }
}
