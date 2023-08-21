namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

public class Level3
{
    public int Id { get; set; }
    public int ParentId { get; set; }

    public Level4 Child4 { get; set; }
}
