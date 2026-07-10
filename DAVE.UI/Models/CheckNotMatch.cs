using System;
using System.Linq;

namespace DAVE.Models;

public class CheckNotMatch : CheckBase
{
    private readonly bool _pass;

    public override bool Pass => _pass;

    public CheckNotMatch(int number, string name, string current, string? previous, string match, string queryMessage) :
        base(number, name, current, previous, queryMessage)
    {
        _pass = !string.Equals(current, match, StringComparison.InvariantCultureIgnoreCase);
    }

    public CheckNotMatch(int number, string name, string current, string? previous, string[] matches,
        string queryMessage)
        : base(number, name, current, previous, queryMessage)
    {
        _pass = !matches.Any(m => current.Contains(m, StringComparison.InvariantCultureIgnoreCase));
    }
}
