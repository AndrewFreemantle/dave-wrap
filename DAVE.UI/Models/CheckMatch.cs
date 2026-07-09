using System;

namespace DAVE.Models;

public class CheckMatch : CheckBase
{
    private readonly string _match;

    public override bool Pass => !string.IsNullOrWhiteSpace(Current) && string.Equals(Current, _match, StringComparison.InvariantCultureIgnoreCase);

    public CheckMatch(int number, string name, string current, string? previous, string match, string queryMessage) :
        base(number, name, current, previous, queryMessage)
    {
        _match = match;
    }
}
