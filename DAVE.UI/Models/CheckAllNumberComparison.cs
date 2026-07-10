using System;
using System.Collections.Generic;
using System.Linq;

namespace DAVE.Models;

public class CheckAllNumberComparison : CheckBase
{
    private readonly IEnumerable<decimal> _current;
    private readonly IEnumerable<decimal>? _previous;
    private readonly int _percentageDifferenceAllowed;

    public override bool Pass
    {
        get
        {
            if (_previous == null)
                return true;    // nothing to compare against, return a Pass

            if (_current.Count() != _previous.Count())
                throw new ArgumentException("Arrays of decimals must have the same length.");

            var percentageDifferenceAllowed = _percentageDifferenceAllowed / 100m;

            for (int i = 0; i < _current.Count(); i++)
            {
                var one = _current.ElementAt(i);
                var two = _previous.ElementAt(i);

                var min = Math.Min(one, two);
                var max = Math.Max(one, two);

                if (min == 0)
                    if (max == 0) // going from/to zero is an infinite percentage change, unless both are zero
                        continue;
                    else
                        return false;

                var percentageDifference = (max - min) / min;
                if (percentageDifference >= percentageDifferenceAllowed)
                    return false;
            }

            return true;
        }
    }

    public CheckAllNumberComparison(int number, string name, IEnumerable<decimal> current, IEnumerable<decimal>? previous, int percentageDifferenceAllowed,
        string queryMessage)
        : base(number, name, $"{current.Count()} values checked", previous != null ? $"{previous.Count()} values checked" : string.Empty, queryMessage)
    {
        _current = current;
        _previous = previous;
        _percentageDifferenceAllowed = percentageDifferenceAllowed;
    }
}
