using System;

namespace DAVE.Models;

public class CheckFLWReductionTarget : CheckBase
{
    private readonly string _target;
    private readonly string _targetForm;
    private readonly string _baselineYear;
    private readonly string _targetYear;
    private readonly string _percentage;

    public override bool Pass
    {
        get
        {
            if (!string.IsNullOrEmpty(_target) && _target.StartsWith("Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                // target set
                // is it year-on-year?
                if (!string.IsNullOrEmpty(_targetForm) && _targetForm.StartsWith("Year", StringComparison.InvariantCultureIgnoreCase))
                    return (!string.IsNullOrEmpty(_baselineYear) || !string.IsNullOrEmpty(_targetYear));  // need at least one of these

                // not year-on-year: we need all values
                return !string.IsNullOrEmpty(_target)
                       && !string.IsNullOrEmpty(_baselineYear)
                       && !string.IsNullOrEmpty(_targetYear)
                       && !string.IsNullOrEmpty(_percentage);
            }

            // if the response isn't "Yes" then this check passes..
            return true;
        }
    }

    public CheckFLWReductionTarget(int number, string name, string current, string? previous, string targetForm, string baselineYear, string targetYear, string percentage, string queryMessage)
        : base(number, name, current, previous, queryMessage)
    {
        _target = current;
        _targetForm = targetForm;
        _baselineYear = baselineYear;
        _targetYear = targetYear;
        _percentage = percentage;
    }
}
