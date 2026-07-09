using System;

namespace DAVE.Models;

public class CheckFoodPoMReported : CheckBase
{
    private readonly string[] _allowedUnits = ["", "tonnes", "n/a"];

    private readonly decimal _currentTonnes;
    private readonly string _currentUnits;


    public override bool Pass {
        get
        {
            if (_currentTonnes > 0 && _allowedUnits.Contains(_currentUnits, StringComparer.InvariantCultureIgnoreCase))
                return true;

            if (_currentTonnes == 0)
                return !_allowedUnits.Contains(_currentUnits, StringComparer.InvariantCultureIgnoreCase);

            return false;
        }
    }

    public CheckFoodPoMReported(int number, string name, decimal currentTonnes, string currentUnits, decimal? previousTonnes, string? previousUnits, string queryMessage) : base(
        number,
        name,
        $"{currentTonnes:N}, {currentUnits}", previousTonnes.HasValue ? $"{previousTonnes:N}, {previousUnits}" : string.Empty, queryMessage)
    {
        _currentTonnes = currentTonnes;
        _currentUnits = currentUnits;
    }
}
