using System;
using System.Collections.Generic;
using System.Linq;

namespace DAVE.Models;

public class CheckDataSharing : CheckBase
{
    private readonly IEnumerable<string> _permissions;
    private readonly string _currentApproval;
    private readonly string _permissionGiven;
    private readonly string _approvalGranted;

    public override bool Pass => _permissions.Any(p => string.Equals(p.Trim(), _permissionGiven, StringComparison.InvariantCultureIgnoreCase))
                                 && string.Equals(_currentApproval.Trim(), _approvalGranted, StringComparison.InvariantCultureIgnoreCase);

    public CheckDataSharing(int number, string name, IEnumerable<string> permissions, string currentApproval, string? previousApproval, string permissionGiven, string approvalGranted, string queryMessage)
        : base(number, name, approvalGranted, previousApproval, queryMessage)
    {
        _permissions = permissions;
        _currentApproval = currentApproval;
        _permissionGiven = permissionGiven;
        _approvalGranted = approvalGranted;
    }
}
