using System;
using System.Numerics;

namespace DAVE.Models;

public class CheckGreaterOrEqual<T> : CheckBase where T: INumber<T>
{
    private readonly INumber<T> _lhs;
    private readonly INumber<T> _rhs;

    public override bool Pass => _lhs.CompareTo(_rhs) >= 0;

    public CheckGreaterOrEqual(int number, string name, T lhs, T rhs, string queryMessage) : base(number, name, $"{lhs} >= {rhs}", null, queryMessage)
    {
        _lhs = lhs;
        _rhs = rhs;
    }
}
