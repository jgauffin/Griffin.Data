namespace Griffin.Data.Tests.Subjects;

internal class SharedMainCollection
{
    public int Id { get; set; }
    public IReadOnlyList<SharedChild> Left { get; set; } = null!;
    public IReadOnlyList<SharedChild> Right { get; set; } = null!;
    public int SomeField { get; set; }
}