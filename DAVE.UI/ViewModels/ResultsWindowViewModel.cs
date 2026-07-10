using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DAVE.Models;
using DAVE.Services;

namespace DAVE.ViewModels;

public partial class ResultsWindowViewModel() : ViewModelBase
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
        Results.Add(new CheckGreaterOrEqual<int>(12, "Sites Covered Check",
            CurrentSheet.GetValue<int>(DataFieldName.SitesTotal),
            CurrentSheet.GetValue<int>(DataFieldName.SitesCovered),
            "Rows 26-27: Sites covered by report (Row 27) exceeds total number of sites (Row 26). Total sites must be greater than or equal to sites covered by report."));
        Results.Add(new CheckGreaterOrEqual<int>(13, "Sites Contributing Check",
            CurrentSheet.GetValue<int>(DataFieldName.SitesCovered),
            CurrentSheet.GetValue<int>(DataFieldName.SitesContributing),
            "Rows 27-28: Sites contributing data (Row 28) exceeds sites covered by report (Row 27). Sites covered by report must be greater than or equal to sites contributing data."));
        Results.Add(new CheckSiteChange(13, "Sites Changed",
            CurrentSheet.GetValue<int>(DataFieldName.SitesTotal),
            PreviousSheet?.GetValue<int>(DataFieldName.SitesTotal), "Row 26: A noticeable year-on-year change in total number of sites has been identified. Please ensure you have provided an explanation for this change (e.g. extending the scope of reporting, acquisitions, closing of sites, errors in previous submission.)"));
        Results.Add(new CheckSiteChange(14, "Sites Covered Changed",
            CurrentSheet.GetValue<int>(DataFieldName.SitesCovered),
            PreviousSheet?.GetValue<int>(DataFieldName.SitesCovered), "Row 27: A significant year-on-year change in sites covered by report has been identified. Please ensure you have provided an explanation for this change (e.g. extending the scope of reporting, acquisitions, closing of sites, errors in previous submission.)"));
        Results.Add(new CheckSiteChange(15, "Sites Contributing Changed",
            CurrentSheet.GetValue<int>(DataFieldName.SitesContributing),
            PreviousSheet?.GetValue<int>(DataFieldName.SitesContributing), "Row 28: A significant year-on-year change in sites directly contributing data has been identified. Please ensure you have provided an explanation for this change (e.g. improvements to data availability or quality, updated measurement systems, or changes in operations.)"));
        Results.Add(new CheckIfGiven(16, "Tonnes of Food Produced",
            CurrentSheet.GetValue<string>(DataFieldName.TonnesOfFoodProduced),
            PreviousSheet?.GetValue<string>(DataFieldName.TonnesOfFoodProduced), "Row 29: Incomplete response. Please provide tonnes of food sold as intended / placed on the market. If this figure is not available, please explain why within your submission."));
        Results.Add(new CheckNumberComparison(17, "Tonnes of Food Changed?",
            CurrentSheet.GetValue<decimal>(DataFieldName.TonnesOfFoodProduced),
            PreviousSheet?.GetValue<decimal>(DataFieldName.TonnesOfFoodProduced), 10,
            "Row 29: A significant change in the tonnes of food sold as intended / placed on the market has been identified. This may have also impacted your food waste as a % of food handled (FLW%, Row 50). Please ensure you have included an explanation for this change within your submission (Row 149) (e.g. business growth or contraction, aquisitions or divestments, changes to reporting boundary etc.)."));
        Results.Add(new CheckFoodPoMReported(18, "Food PoM Reported",
            CurrentSheet.GetValue<decimal>(DataFieldName.TonnesOfFoodProduced),
            CurrentSheet.GetValue<string>(DataFieldName.UnitsProduced),
            PreviousSheet?.GetValue<decimal>(DataFieldName.TonnesOfFoodProduced),
            PreviousSheet?.GetValue<string>(DataFieldName.UnitsProduced),
            "Row 29-30: Food sold as intended / placed on the market must be reported in tonnes using Row 29.\nIf Row 29 is populated, please confirm that the value is in tonnes. If it is not tonnes, leave Row 29 blank and report the value and unit in Row 30 instead.\n"));
        Results.Add(new CheckHaFSBusiness(19, "HaFS Business",
            CurrentSheet.GetValue<string>(DataFieldName.Sector),
            CurrentSheet.GetValue<string>(DataFieldName.HaFSTotalAnnualCovers),
            PreviousSheet?.GetValue<string>(DataFieldName.Sector),
            PreviousSheet?.GetValue<string>(DataFieldName.HaFSTotalAnnualCovers),
            "Row 31: Incomplete response. As a food service and hospitality business, please provide your total annual number of covers for the reporting period."));
        Results.Add(new CheckIfGiven(20, "Packaging Weight",
            CurrentSheet.GetValue<string>(DataFieldName.PackagingWeight),
            PreviousSheet?.GetValue<string>(DataFieldName.PackagingWeight),
            "Row 32: Incomplete response. Please indicate whether you have excluded packaging weight from the tonnage figures provided. \n\nPlease note that packaging weight should be excluded from the following tonnage values: food sold as intended (row 29), food waste destinations (rows 39-48) and other destinations (rows 59-62). \n\nIf you are able to estimate packaging weight within tonnages provided, please re-submit figures with packaging weight removed. Please advise if this estimate is based on product, business or sector knowledge?\nIf you are unable to estimate packaging weight, a 15% packaging weight assumption should be applied (WRAP industry estimate), however, more sector-specific packaging weight estimates are available.\n\nIt's also recommended that you explore ways to calculate a more robust figure excluding packaging weight (more guidance can be provided).",
            ["N/A"]));
        Results.Add(new CheckMatch(21, "Packaging Weight Excluded",
            CurrentSheet.GetValue<string>(DataFieldName.PackagingWeight),
            PreviousSheet?.GetValue<string>(DataFieldName.PackagingWeight),
            "No",
            "Row 32: You have identified that packaging weight has not been excluded. Please note that packaging weight should be excluded from the following tonnage values: food sold as intended (row 29), food waste destinations (rows 39-48) and other destinations (rows 59-62). \n\nIf you are able to estimate packaging weight within tonnages provided, please re-submit figures with packaging weight removed. Please advise if this estimate is based on product, business or sector knowledge?\nIf you are unable to estimate packaging weight, a 15% packaging weight assumption should be applied (WRAP industry estimate), however, more sector-specific packaging weight estimates are available.\n\nIt's also recommended that you explore ways to calculate a more robust figure excluding packaging weight (more guidance can be provided)."));


        OnPropertyChanged(nameof(IsEmailEnabled));
        OnPropertyChanged(nameof(ResultsStats));
    }
}
