namespace Griffin.Data.Tests.Subjects;

internal class SharedMain
{
    public int Id { get; set; }
    public SharedChild Left { get; set; } = null!;
    public SharedChild Right { get; set; } = null!;
    public int SomeField { get; set; }
}

internal class SharedMainCollection
{
    public int Id { get; set; }
    public IReadOnlyList<SharedChild> Left { get; set; } = null!;
    public IReadOnlyList<SharedChild> Right { get; set; } = null!;
    public int SomeField { get; set; }
}

public class SharedChild
{
    public int Id { get; set; }
    public int MainId { get; set; }
    public string Value { get; set; } = "";
}
