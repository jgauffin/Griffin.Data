namespace Griffin.Data.Tests.Subjects;

internal class SharedMain
{
    public int Id { get; set; }
    public SharedChild Left { get; set; } = null!;
    public SharedChild Right { get; set; } = null!;
    public int SomeField { get; set; }
}