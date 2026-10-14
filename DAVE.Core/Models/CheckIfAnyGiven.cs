using System;
using System.Collections.Generic;
using System.Linq;

namespace DAVE.Models;

public class CheckIfAnyGiven : CheckBase
{
    private readonly IEnumerable<string> _currentValues;
    private readonly string[]? _considerAsNotGiven;

    public override bool Pass => CountValuesGiven(_currentValues, _considerAsNotGiven) > 0;

    public CheckIfAnyGiven(int number,
        string name,
        IEnumerable<string> current,
        IEnumerable<string>? previous,
        string queryResult,
        string[]? considerAsNotGiven = null)
        : base(number,
            name,
            $"{CountValuesGiven(current, considerAsNotGiven)} responses",
            previous != null ? $"{CountValuesGiven(previous, considerAsNotGiven)} responses" : null,
            queryResult)
    {
        _currentValues = current;
        _considerAsNotGiven = considerAsNotGiven;
    }

    private static int CountValuesGiven(IEnumerable<string> values, string[]? considerAsNotGiven)
    {
        return considerAsNotGiven != null
            ? values.Count(v => !string.IsNullOrWhiteSpace(v) && !considerAsNotGiven.Contains(v))
            : values.Count(v => !string.IsNullOrWhiteSpace(v));
    }
}
