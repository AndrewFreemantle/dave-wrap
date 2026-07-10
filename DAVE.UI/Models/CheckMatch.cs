using System;

namespace DAVE.Models;

public class CheckMatch : CheckBase
{
    private readonly string _current;
    private readonly string _match;

    public override bool Pass => IsMatch(_current, _match);

    public CheckMatch(int number, string name, string current, string? previous, string match, string queryMessage) :
        base(number, name, current, previous, queryMessage)
    {
        _current = current;
        _match = match;
    }

    private static bool IsMatch(string value, string match) => !string.IsNullOrWhiteSpace(value) && string.Equals(value, match, StringComparison.InvariantCultureIgnoreCase);
}
