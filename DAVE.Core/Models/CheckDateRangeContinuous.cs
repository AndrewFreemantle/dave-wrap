using System;

namespace DAVE.Models;

public class CheckDateRangeContinuous : CheckDateRange
{
    private readonly DateTime? _previousStart;
    private readonly DateTime? _previousEnd;

    public override bool Pass
    {
        get
        {
            // nothing to compare against: pass
            if (!_previousStart.HasValue || !_previousEnd.HasValue)
                return true;

            // continuousness and overlap check
            var diff = _start - _previousEnd.Value;
            if (Math.Abs(diff.TotalDays) > _daysAllowed)
                return false;

            return true;
        }
    }

    public CheckDateRangeContinuous(int number,
        string name,
        DateTime start,
        DateTime end,
        DateTime? previousStart,
        DateTime? previousEnd,
        int daysAllowed,
        string queryMessage)
        : base(number, name, start, end, daysAllowed, queryMessage)
    {
        _previousStart = previousStart;
        _previousEnd = previousEnd;

        Previous = previousStart.HasValue
            ? $"{previousStart:d}-{previousEnd:d}"
            : string.Empty;
    }

}
