namespace DAVE.Models;

public class CheckIfGiven : CheckBase
{
    public override bool Pass => !string.IsNullOrWhiteSpace(Current);

    public CheckIfGiven(int number, string name, string current, string? previous, string queryMessage) : base(number, name, current, previous, queryMessage) { }
}
