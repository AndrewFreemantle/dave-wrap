
namespace DAVE.Models;

public class CheckBase(int number, string name, string current, string? previous, string queryMessage)
{
    public int Number { get; set; } = number;
    public string Name { get; set; } = name;
    public string Current { get; set; } = current;
    public string? Previous { get; set; } = previous;
    public string QueryMessage { get; set; } = queryMessage;
    public virtual bool Pass { get; } = false;
}
