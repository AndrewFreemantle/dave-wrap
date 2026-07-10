using System;

namespace DAVE.Models;

public class CheckIfGiven : CheckBase
{
    // count the following strings as Given (i.e. Pass)
    private readonly string[]? _countAsGiven;

    public override bool Pass =>
        _countAsGiven == null
            ? !string.IsNullOrWhiteSpace(Current)
            : !string.IsNullOrWhiteSpace(Current) || _countAsGiven.Contains(Current, StringComparer.InvariantCultureIgnoreCase);

    public CheckIfGiven(
        int number,
        string name,
        string current,
        string? previous,
        string queryMessage,
        string[]? countAsGiven = null)
        : base(number, name, current, previous, queryMessage)
    {
        _countAsGiven = countAsGiven;
    }
}
