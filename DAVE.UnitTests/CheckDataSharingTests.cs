using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckDataSharingTests
{
    private const string PermissionGiven = "Yes I give permission for the data to be shared";
    private const string ApprovalGranted = "Yes, the appropriate permission has been sought and granted";
    private const string Query = "query";

    [Fact]
    public void Passes_when_no_permission_was_offered_and_no_approval_given()
    {
        var check = new CheckDataSharing(44, "Data Sharing",
            ["No", "No", ""], "No", null, PermissionGiven, ApprovalGranted, Query);

        Assert.True(check.Pass);
    }

    [Fact]
    public void Passes_when_permission_offered_and_approval_granted()
    {
        var check = new CheckDataSharing(44, "Data Sharing",
            [PermissionGiven, "No"], ApprovalGranted, null, PermissionGiven, ApprovalGranted, Query);

        Assert.True(check.Pass);
    }

    [Fact]
    public void Fails_when_permission_offered_but_approval_not_granted()
    {
        var check = new CheckDataSharing(44, "Data Sharing",
            [PermissionGiven, "No"], "No", null, PermissionGiven, ApprovalGranted, Query);

        Assert.False(check.Pass);
    }
}
