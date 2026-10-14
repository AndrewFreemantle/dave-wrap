using DAVE.Models;
using DAVE.ViewModels;

namespace DAVE.UnitTests;

public class CheckSetTests
{
    // The checks as ResultsWindowViewModel.Verify() registered them before they moved to CheckSet (main @ f059263).
    //  Hard-coded so that moving or editing the registrations cannot silently reorder them.
    //  Check numbers 13 and 24 appear twice, and 22 and 28 are not used, exactly as the results grid shows them
    private static readonly (Type Type, int Number, string Name)[] RegistrationOrder =
    [
        (typeof(CheckIfGiven), 1, "Company Name"),
        (typeof(CheckIfGiven), 2, "Annual Turnover"),
        (typeof(CheckNumberComparison), 3, "Annual Turnover Comparable?"),
        (typeof(CheckDateRange), 4, "Inventory 12 months"),
        (typeof(CheckDateRangeContinuous), 5, "Inventory Continuous"),
        (typeof(CheckMatch), 6, "United Kingdom"),
        (typeof(CheckIfGiven), 7, "Business Sector"),
        (typeof(CheckIfGiven), 8, "Lifecycle"),
        (typeof(CheckIfGiven), 9, "Sites Total"),
        (typeof(CheckIfGiven), 10, "Sites Covered"),
        (typeof(CheckIfGiven), 11, "Sites Contributing"),
        (typeof(CheckGreaterOrEqual<int>), 12, "Sites Covered Check"),
        (typeof(CheckGreaterOrEqual<int>), 13, "Sites Contributing Check"),
        (typeof(CheckSiteChange), 13, "Sites Changed"),
        (typeof(CheckSiteChange), 14, "Sites Covered Changed"),
        (typeof(CheckSiteChange), 15, "Sites Contributing Changed"),
        (typeof(CheckIfGiven), 16, "Tonnes of Food Produced"),
        (typeof(CheckNumberComparison), 17, "Tonnes of Food Changed?"),
        (typeof(CheckFoodPoMReported), 18, "Food PoM Reported"),
        (typeof(CheckHaFSBusiness), 19, "HaFS Business"),
        (typeof(CheckIfGiven), 20, "Packaging Weight"),
        (typeof(CheckMatch), 21, "Packaging Weight Excluded"),
        (typeof(CheckIfAllGiven), 23, "FLW Data Yes/No/Unsure?"),
        (typeof(CheckAllNumberComparison), 24, "FLW Data Changed?"),
        (typeof(CheckGreaterOrEqual<decimal>), 25, "Wastewater / Total FLW %"),
        (typeof(CheckNotMatch), 26, "Inedible = FLW"),
        (typeof(CheckIfAnyGiven), 27, "Material Sent Elsewhere?"),
        (typeof(CheckAllNumberComparison), 24, "Material Sent Elsewhere Changed?"),
        (typeof(CheckNotMatch), 29, "Redistribution Notes"),
        (typeof(CheckNotMatch), 30, "Bio-redistribution Notes"),
        (typeof(CheckIfGiven), 31, "FLW Reduction Target?"),
        (typeof(CheckNotMatch), 32, "FLW Reduction Target: Yes?"),
        (typeof(CheckFLWReductionTarget), 33, "FLW Reduction Targets"),
        (typeof(CheckMatch), 34, "FLW Target Achieved?"),
        (typeof(CheckIfGiven), 35, "FLW Target Form?"),
        (typeof(CheckMatch), 36, "FLW Target Year-on-Year?"),
        (typeof(CheckIfGiven), 37, "FLW Progress"),
        (typeof(CheckFLWReductionEfforts), 38, "FLW Reduction Efforts"),
        (typeof(CheckIfAllGiven), 39, "Method & Frequency"),
        (typeof(CheckSiteExclusions), 40, "Site Exclusions"),
        (typeof(CheckSiteExclusions), 41, "Site Exclusion Reason"),
        (typeof(CheckIfGiven), 42, "Based on Principles?"),
        (typeof(CheckMatch), 43, "Principles: No/Unsure?"),
        (typeof(CheckDataSharing), 44, "Data Sharing"),
        (typeof(CheckMatch), 45, "Data Reporting"),
    ];

    private static readonly DataCaptureSpreadsheet CurrentSheet = new(DataCaptureSheetBuilder.Current().Build());
    private static readonly DataCaptureSpreadsheet PreviousSheet = new(DataCaptureSheetBuilder.Previous().Build());

    [Fact]
    public void All_WithPrevious_Returns45Checks_InRegistrationOrder()
    {
        var checks = CheckSet.All(CurrentSheet, PreviousSheet);

        Assert.Equal(45, checks.Count);
        Assert.Equal(RegistrationOrder, checks.Select(c => (c.GetType(), c.Number, c.Name)).ToArray());
    }

    [Fact]
    public void All_WithoutPrevious_Returns45Checks_InRegistrationOrder()
    {
        var checks = CheckSet.All(CurrentSheet, null);

        Assert.Equal(45, checks.Count);
        Assert.Equal(RegistrationOrder, checks.Select(c => (c.GetType(), c.Number, c.Name)).ToArray());
    }

    [Fact]
    public void Verify_ShowsTheSameResultsAsCheckSet()
    {
        var viewModel = new ResultsWindowViewModel(CurrentSheet, PreviousSheet);
        viewModel.Verify();

        var expected = CheckSet.All(CurrentSheet, PreviousSheet)
            .Select(c => (c.GetType(), c.Number, c.Name, c.Pass, c.Current, c.Previous, c.QueryMessage));
        var shown = viewModel.Results
            .Select(c => (c.GetType(), c.Number, c.Name, c.Pass, c.Current, c.Previous, c.QueryMessage));

        Assert.Equal(expected, shown);
    }
}
