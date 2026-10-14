using System;
using System.Collections.Generic;
using System.Linq;

namespace DAVE.Models;

public class CheckFLWReductionEfforts : CheckBase
{
    private readonly IEnumerable<string> _dropDownValues;
    private readonly IEnumerable<string> _notes;

    private readonly IEnumerable<string>? _previousDropDownValues;
    private readonly IEnumerable<string>? _previousNotes;

    public override bool Pass
    {
        get
        {
            // Option must be selected from drop-down menu
            if (_dropDownValues.Any(string.IsNullOrWhiteSpace))
                return false;

            // Where a 'Yes' response has been provided, evidence must be provided
            for (int i = 0; i < _dropDownValues.Count(); i++)
            {
                if (string.Equals(_dropDownValues.ElementAt(i), "Yes", StringComparison.InvariantCultureIgnoreCase)
                    && string.IsNullOrWhiteSpace(_notes.ElementAt(i)))
                    return false;
            }

            return true;
        }
    }


    public CheckFLWReductionEfforts(int number,
        string name,
        IEnumerable<string> dropDownValues,
        IEnumerable<string> notes,
        IEnumerable<string>? previousDropDownValues,
        IEnumerable<string>? previousNotes,
        string queryMessage)
        : base(number, name, string.Empty, string.Empty, queryMessage)
    {
        _dropDownValues = dropDownValues;
        _notes = notes;
        _previousDropDownValues = previousDropDownValues;
        _previousNotes = previousNotes;

        Current = $"{dropDownValues.Count(v => !string.IsNullOrWhiteSpace(v))}/4 Responses";
        Previous = previousDropDownValues != null ? $"{previousDropDownValues.Count()}/4 Responses" : string.Empty;
    }
}
