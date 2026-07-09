namespace DAVE.Models;

public class CheckCustom : CheckBase
{
    public override bool Pass { get; }

    public CheckCustom(int number, string name, string current, string? previous, bool pass, string queryMessage) :
        base(number, name, current, previous, queryMessage)
    {
        Pass = pass;
    }
}
