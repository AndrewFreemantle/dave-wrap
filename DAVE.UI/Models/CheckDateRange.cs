using System;

namespace DAVE.Models;

public class CheckDateRange : CheckBase
{
    protected readonly DateTime _start;
    protected readonly DateTime _end;
    protected readonly int _daysAllowed;

    public override bool Pass
    {
        get
        {
            var diff = _end - _start;
            var driftFromAFullYear = Math.Abs(364 - diff.TotalDays);
            return driftFromAFullYear <= _daysAllowed;
        }
    }

    public CheckDateRange(int number, string name, DateTime start, DateTime end, int daysAllowed, string queryMessage) : base(number, name, $"{start:d}-{end:d}", string.Empty, queryMessage)
    {
        _start = start;
        _end = end;
        _daysAllowed = daysAllowed;
    }
}
