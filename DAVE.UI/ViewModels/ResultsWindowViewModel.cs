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

        // ## Company Information
        Results.Add(new CheckIfGiven(1, "Company Name",     CurrentSheet.GetValue<string>(DataFieldName.CompanyName), PreviousSheet?.GetValue<string>(DataFieldName.CompanyName), "Row 8: Incomplete response. Please confirm the Company Name to which the data capture sheet refers"));
        Results.Add(new CheckIfGiven(2, "Annual Turnover",  CurrentSheet.GetValue<string>(DataFieldName.AnnualTurnover), PreviousSheet?.GetValue<string>(DataFieldName.AnnualTurnover), "Row 13: New requirement of updated form - Annual turnover missing. Please could you provide? This is required purely for categorising the size of the business and is not shared."));
        Results.Add(new CheckNumberComparison(3, "Annual Turnover Comparable?", CurrentSheet.GetValue<decimal>(DataFieldName.AnnualTurnover), PreviousSheet?.GetValue<decimal>(DataFieldName.AnnualTurnover), 20, "Row 13: There appears to be a significant change in your turnover. Please clarify why this is the case.", true));

        // ## Scope of the FLW Inventory
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

        // ## Data Summary
        Results.Add(new CheckIfAllGiven(23, "FLW Data Yes/No/Unsure?",
            CurrentSheet.GetValues<string>([39, 40, 41, 42, 43, 44, 45, 46, 47, 48], [2]),
            PreviousSheet?.GetValues<string>([39, 40, 41, 42, 43, 44, 45, 46, 47, 48], [2]),
            "Rows 39-48: One or more incomplete cells have been identified. Please complete Columns B with \"Yes\", \"No\" or \"Unsure\".",
            ["Yes", "No", "Unsure"]));
        Results.Add(new CheckAllNumberComparison(24, "FLW Data Changed?",
            CurrentSheet.GetValues<decimal>([39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49], [2]),
            PreviousSheet?.GetValues<decimal>([39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49], [2]),
            10,
            "Rows 39-48: A significant year-on-year change in food waste destinations or total FLW has been identified. This may have also impacted your food waste as a % of food handled (FLW%, Row 50). Please ensure you have included an explanation for this change within the submission notes (Column E or Row 149) e.g. operational changes, expanding scope, updated methodology etc)."));
        Results.Add(new CheckNumberComparison(25, "Wastewater / Total FLW %",
            CurrentSheet.GetValue<decimal>(DataFieldName.SewerWastewaterTreatment),
            CurrentSheet.GetValue<decimal>(DataFieldName.TotalFLW),
            10,
            "Row 44: The tonnage of food waste sent to sewer / wastewater treatment has been identified as unusually high. Please review the tonnage provided and confirm the value reported is the food element (suspended solids/ sludge) contained within the wastewater and not the tonnage of wastewater treated (please include details in Column E).",
            passIfOneIsBlankOrZero: true,
            twoIsPrevious: false));
        Results.Add(new CheckNotMatch(26, "Inedible = FLW",
            CurrentSheet.GetValue<string>(DataFieldName.FoodVsInediblePartsNotice),
            PreviousSheet?.GetValue<string>(DataFieldName.FoodVsInediblePartsNotice),
            "Warning: estimate of food and inedible parts does not equal total FLW; please amend",
            "Rows 55-56: The total estimate of food and inedible parts does not equal the total FLW value in cell C49. Please review and amend."));
        Results.Add(new CheckIfAnyGiven(27, "Material Sent Elsewhere?",
            CurrentSheet.GetValues<string>([59, 60, 61, 62], [2, 3, 5]),
            PreviousSheet?.GetValues<string>([59, 60, 61, 62], [2, 3, 5]),
            "Rows 59-62: Please complete Columns B,C and E and resubmit."));
        Results.Add(new CheckAllNumberComparison(24, "Material Sent Elsewhere Changed?",
            CurrentSheet.GetValues<decimal>([59, 60, 61, 62, 63], [2]),
            PreviousSheet?.GetValues<decimal>([59, 60, 61, 62, 63], [2]),
            10,
            "Rows 59-63: A significant year-on-year change in food sent to other destinations and / or total food sent to other destinations (food surplus) has been identified. This may have also impacted your food surplus as a % of food handled (% in Row 63). Please ensure you have included an explanation for this change within the submission notes (Column E or Row 149) e.g. operational changes, expanding scope, updated methodology etc)."));
        Results.Add(new CheckNotMatch(29, "Redistribution Notes",
            CurrentSheet.GetValue<string>(DataFieldName.RedistributionNotes),
            PreviousSheet?.GetValue<string>(DataFieldName.RedistributionNotes),
            ["Too Good To Go", "TGTG", "third party apps", "Staff sales", "Staff shop", "in-store discount", "yellow sticker"],
            "Row 59: The tonnage of surplus food sent to redistribution may contain food that has been sold and does not qualify as redistribution.\n\nTo note: Food that is sold at reduced price in-store (e.g. discounted 'yellow sticker' sales), through staff sales/shop, or via a food waste app such as Too Good To Go is out of scope and should be excluded. Food sold via any of these named destinations should be included in the food sold as intended / placed on the market figure (Row 29).\nIn the context of FLW prevention, only include redistributed surplus food where the food would otherwise have ended up as FLW, or would have been sent to one of the Other Destinations. This may include food redistributed by both charitable organisations (such as FareShare, Food Cycle) and commercial ones (such as Company Shop, who also operate Community Shop).\n\nPlease review your redistribution figure and ensure you have followed the guidance above correctly."));
        Results.Add(new CheckNotMatch(30, "Bio-redistribution Notes",
            CurrentSheet.GetValue<string>(DataFieldName.BioRedistributionNotes),
            PreviousSheet?.GetValue<string>(DataFieldName.BioRedistributionNotes),
            ["coffee grounds", "spent grain", "cooking oil", "oil", "biofuel", "biodiesel", "fuel pellets", "fuel logs"],
            "Row 61: The tonnage of food surplus sent to biomaterials may be miscategorised. \n\nMaterials such as coffee grounds, spent grain, cooking oil etc, or other similar materials sent for processing into biofuels (e.g biodiesel or fuel logs/pellets) should be reported in Food Waste destinations under \"Other\" Row 46.\n\nPlease review your entry and amend if required."));

        // ## Data Summary
        Results.Add(new CheckIfGiven(31, "FLW Reduction Target?",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTarget),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTarget),
            "Row 75: Incomplete response. You have not indicated whether your company has set a FLW reduction target.\n\nTo be fully compliant with the Roadmap every business needs to set a FW reduction target, ideally within 6-12 months of committing to the Roadmap. Best practice is to set a baseline year, a target year and % reduction target, with the target taking the form of \"50% reduction by 2030 compared with a baseline of 2021\". Please review and confirm.\n\nIf you are not in a position to state your reduction target then please select the response that most closely reflects your organisation's current position."));
        Results.Add(new CheckNotMatch(32, "FLW Reduction Target: Yes?",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTarget),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTarget),
            ["No, but we are currently working on this", "No, but we plan to work on this next year", "No"],
            "Rows 75: You have indicated that your company has not set a FLW reduction target.\n\nTo be fully compliant with the Roadmap every business needs to set a FW reduction target, ideally within 6-12 months of committing to the Roadmap. Best practice is to set a baseline year, a target year and % reduction target, with the target taking the form of \"50% reduction by 2030 compared with a baseline of 2021\". Please provide details of the steps being taken to implement a company FLW reduction target in the Notes (Row 75, Column D)."));
        Results.Add(new CheckFLWReductionTarget(33, "FLW Reduction Targets",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTarget),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTarget),
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTargetForm),
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionBaselineYear),
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTargetYear),
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionPercentage),
            "Rows 75-79: Incomplete response. You have indicated that you have set a FLW reduction target, but not provided details of this target.\n\nPlease ensure a baseline year, target year and % reduction target has been provided where appropriate."));
        Results.Add(new CheckMatch(34, "FLW Target Achieved?",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTarget),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTarget),
            "Yes, but target has been achieved",
            "Row 75: You have indicated that you have set a FLW reduction target, but that this target has been achieved. Please provide details on whether your organisation has considered revising the original target e.g., increasing the % reduction target, or setting an alternative target to focus on other areas of FLW e.g., redistribution or animal feed (include within the Notes, Row 149)."));
        Results.Add(new CheckIfGiven(35, "FLW Target Form?",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTargetForm),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTargetForm),
            "Row 76: Drop-down option not selected. Please indicate what form your FLW reduction target takes. If you have not yet set a FLW reduction target, please select the option that best describes your progress against this."));
        Results.Add(new CheckMatch(36, "FLW Target Year-on-Year?",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionTargetForm),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionTargetForm),
            "Year on year target (no fixed baseline/target years)",
            "Row 76: You have indicated that a year-on-year FLW reduction target has been set. Please provide an explanation on whether your organisation has considered implementing a FLW reduction target that aligns with SDG 12.3 e.g., 50% reduction by 2030 vs set baseline year, and what barriers there are to adopting this form of target (include within the Notes, Row 149).",
            false));
        Results.Add(new CheckIfGiven(37, "FLW Progress",
            CurrentSheet.GetValue<string>(DataFieldName.FLWReductionProgress),
            PreviousSheet?.GetValue<string>(DataFieldName.FLWReductionProgress),
            "Row 80: Incomplete response. Please provide a description of the progress made this year in reducing your food waste. This may be quantitative e.g., 10% reduction achieved, or a more qualitative description."));
        Results.Add(new CheckFLWReductionEfforts(38, "FLW Reduction Efforts",
            CurrentSheet.GetValues<string>([81, 82, 83, 84], [3]),
            CurrentSheet.GetValues<string>([81, 82, 83, 84], [4]),
            PreviousSheet?.GetValues<string>([81, 82, 83, 84], [3]),
            PreviousSheet?.GetValues<string>([81, 82, 83, 84], [4]),
            "Rows 81-84: One or multiple incomplete responses. Please complete all drop down boxes and provide a supporting description, where appropriate (e.g. where action is being taken)."));

        // ## Quantification Methods & Uncertainty
        Results.Add(new CheckIfAllGiven(39, "Method & Frequency",
            CurrentSheet.GetValues<string>([94, 95], [2]),
            PreviousSheet?.GetValues<string>([94, 95], [2]),
            "Rows 94-95: Drop down not selected for one or multiple options. WRAP is working to better understand the methods used by businesses to measure their food waste data and the frequency these data are updated. Please select an option from the drop down menu(s).",
            [
                "Estimates based on assumptions or proxy data (e.g. visual estimates, waste factors)",
                "Calculations based on existing business data (e.g. purchases, sales, production data)",
                "Direct measurement of food waste (e.g. weighing or volume)",
                "Automated or technology-enabled measurement (e.g. systems that automatically track waste)",
                ]));
        Results.Add(new CheckSiteExclusions(40, "Site Exclusions",
            CurrentSheet.GetValue<int>(DataFieldName.SitesCovered),
            CurrentSheet.GetValue<int>(DataFieldName.SitesTotal),
            CurrentSheet.GetValue<string>(DataFieldName.SitesExclusionNotes),
            PreviousSheet?.GetValue<string>(DataFieldName.SitesExclusionNotes),
            "Row 97: Incomplete response. You have indicated that some sites have been excluded from the report but have not indicated which sites have been excluded. Please provide details of which sites have been excluded from this report.",
            "[Text] - please list any exclusions from the inventory"));
        Results.Add(new CheckSiteExclusions(41, "Site Exclusion Reason",
            CurrentSheet.GetValue<int>(DataFieldName.SitesCovered),
            CurrentSheet.GetValue<int>(DataFieldName.SitesTotal),
            CurrentSheet.GetValue<string>(DataFieldName.SitesExclusionReasons),
            PreviousSheet?.GetValue<string>(DataFieldName.SitesExclusionReasons),
            "Row 98: Incomplete response. You have indicated that some sites have been excluded from the report but have not indicated the reasons for these exclusions. Please provide details of why some sites or parts of your operations have been excluded from this report.",
            "[Text] - please give reasons for any exclusions from the inventory"));

        // ## Assurance & Declaration
        Results.Add(new CheckIfGiven(42, "Based on Principles?",
            CurrentSheet.GetValue<string>(DataFieldName.ReportingPrinciples),
            PreviousSheet?.GetValue<string>(DataFieldName.ReportingPrinciples),
            "Row 106: Drop down menu not selected. The FLWS Principles are stated on the named tab of the Data Capture Sheet. Please review and update response."));
        Results.Add(new CheckMatch(43, "Principles: No/Unsure?",
            CurrentSheet.GetValue<string>(DataFieldName.ReportingPrinciples),
            PreviousSheet?.GetValue<string>(DataFieldName.ReportingPrinciples),
            ["No", "Unsure"],
            "Row 106: \"No\" or \"Unsure\" selected from the drop down menu when asked if the report is based on the FLW Standard principles. The FLWS Principles of Relevance, Completeness, Consistency, Transparency and Accuracy are stated and defined on the named tab of the Data Capture Sheet.\n\nCan you please review your initial response and clarify if still unsure?",
            false));

        // ## Data Sharing with Retailers
        Results.Add(new CheckDataSharing(44, "Data Sharing",
            CurrentSheet.GetValues<string>([122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132], [3]),
            CurrentSheet.GetValue<string>(DataFieldName.DataSharingApproval),
            PreviousSheet?.GetValue<string>(DataFieldName.DataSharingApproval),
            "Yes I give permission for the data to be shared",
            "Yes, the appropriate permission has been sought and granted",
            "Row 136: Incomplete. You have indicated that you give permission to be shared to selected retailers, but not have not provided the suitable data sharing permissions. Please select from the Dropdown menu."));

        // ## Data Reporting
        Results.Add(new CheckMatch(45, "Data Reporting",
            CurrentSheet.GetValue<string>(DataFieldName.DataReporting),
            PreviousSheet?.GetValue<string>(DataFieldName.DataReporting),
            ["Yes", "No, but this is something we might consider", "No"],
            "Row 144: Incomplete. Please select from the Dropdown menu"));

        OnPropertyChanged(nameof(IsEmailEnabled));
        OnPropertyChanged(nameof(ResultsStats));
    }
}
