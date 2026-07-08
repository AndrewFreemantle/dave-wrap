namespace DAVE.Models;

public class CheckIfBlank : CheckBase
{
    public override bool Pass => !string.IsNullOrWhiteSpace(Current);

    public CheckIfBlank(int number, string name, string current, string? previous, string queryMessage) : base(number, name, current, previous, queryMessage) { }
}
