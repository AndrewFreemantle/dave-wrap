using System;

namespace DAVE.Models;

public class CheckSiteChange : CheckBase
{
    private readonly int _currentSites;
    private readonly int? _previousSites;

    public override bool Pass
    {
        get
        {
            if (!_previousSites.HasValue)
                return true;

            return _currentSites switch
            {
                < 10 => Math.Abs(_currentSites - _previousSites.Value) == 0,
                < 30 => Math.Abs(_currentSites - _previousSites.Value) < 2,
                < 100 => Math.Abs(_currentSites - _previousSites.Value) < 5,
                < 500 => Math.Abs(_currentSites - _previousSites.Value) < 10,
                < 1000 => Math.Abs(_currentSites - _previousSites.Value) < 25,
                _ => Math.Abs(_currentSites - _previousSites.Value) < 50
            };
        }
    }

    public CheckSiteChange(int number, string name, int currentSites, int? previousSites, string queryMessage) : base(number, name, currentSites.ToString(), previousSites.ToString(), queryMessage)
    {
        _currentSites = currentSites;
        _previousSites = previousSites;
    }
}
