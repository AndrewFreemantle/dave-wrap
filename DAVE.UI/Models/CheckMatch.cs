using System;
using System.Linq;

namespace DAVE.Models;

public class CheckMatch : CheckBase
{
    private readonly string _current;
    private readonly string? _match;
    private readonly string[]? _matches;

    private readonly bool _passIfMatch = true;

    public override bool Pass
    {
        get
        {
            var matched = _match != null
                ? IsMatch(_current, _match)
                : _matches?.Any(match => IsMatch(_current, match)) ?? false;

            return _passIfMatch ? matched : !matched;
        }
    }

    public CheckMatch(int number, string name, string current, string? previous, string match, string queryMessage, bool passIfMatch = true) :
        base(number, name, current, previous, queryMessage)
    {
        _current = current;
        _match = match;
        _passIfMatch = passIfMatch;
    }

    public CheckMatch(int number, string name, string current, string? previous, string[] matches, string queryMessage) :
        base(number, name, current, previous, queryMessage)
    {
        _current = current;
        _matches = matches;
    }

    private static bool IsMatch(string value, string match) => !string.IsNullOrWhiteSpace(value) && string.Equals(value, match, StringComparison.InvariantCultureIgnoreCase);
}
