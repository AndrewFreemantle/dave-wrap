using System;

namespace DAVE.Models;

public class CheckSiteExclusions : CheckBase
{
    private readonly int _sitesCovered;
    private readonly int _sitesTotal;
    private readonly string _notes;
    private readonly string _prompt;

    public override bool Pass
    {
        get
        {
            return _sitesCovered >= _sitesTotal
                   ||
                   (!string.IsNullOrEmpty(_notes)
                    && !string.Equals(_notes, _prompt, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public CheckSiteExclusions(int number, string name, int siteCovered, int siteTotal, string notes,
        string? previousNotes, string queryMessage, string prompt)
        : base(number, name, notes, previousNotes, queryMessage)
    {
        _sitesCovered = siteCovered;
        _sitesTotal = siteTotal;
        _notes = notes;
        _prompt = prompt;
    }
}
