using System;
using System.Collections.Generic;
using System.Linq;

namespace DAVE.Models;

public class CheckIfAllGiven : CheckBase
{
    private readonly IEnumerable<string> _currentValues;
    private readonly string[] _allowedValues;

    public override bool Pass => _currentValues.All(v => _allowedValues.Contains(v, StringComparer.InvariantCultureIgnoreCase));

    public CheckIfAllGiven(int number,
        string name,
        IEnumerable<string> current,
        IEnumerable<string>? previous,
        string queryResult,
        string[] allowedValues)
        : base(number,
            name,
            string.Empty,
            string.Empty,
            queryResult)
    {
        _currentValues = current;
        _allowedValues = allowedValues;

        var allowedValuesString = string.Join("/", _allowedValues);

        Current = Pass
            ? $"All {allowedValuesString}"
            : $"{CountValuesMissing(current, allowedValues)} missing";

        if (previous != null)
            Previous = previous.Count() == CountValuesMissing(previous, allowedValues)
                ? $"All {allowedValuesString}"
                : $"{CountValuesMissing(previous, allowedValues)} missing";
    }

    private static int CountValuesMissing(IEnumerable<string> values, string[] allowedValues)
    {
        return values
            .Count(v => !allowedValues.Contains(v, StringComparer.InvariantCultureIgnoreCase));
    }
}
