using System;

namespace DAVE.Models;

public class CheckNumberComparison : CheckBase
{
    private readonly decimal _one;
    private readonly decimal? _two;
    private readonly int _percentageDifferenceAllowed;

    public override bool Pass
    {
        get
        {
            if (!_two.HasValue)
                return true;    // nothing to compare against, return a Pass

            var two = _two.Value;
            var min = Math.Min(_one, two);
            var max = Math.Max(_one, two);

            if (min == 0)
                return max == 0;    // going from/to zero is an infinite percentage change, unless both are zero

            var percentageDifference = (max - min) / min;
            var percentageDifferenceAllowed = _percentageDifferenceAllowed / 100m;

            return percentageDifference <= percentageDifferenceAllowed;
        }
    }

    public CheckNumberComparison(int number, string name, decimal one, decimal? two, int percentageDifferenceAllowed, string queryMessage) : base(number, name, $"{one:C0}", string.Empty, queryMessage)
    {
        _one = one;
        _two = two;
        _percentageDifferenceAllowed = percentageDifferenceAllowed;
    }
}
