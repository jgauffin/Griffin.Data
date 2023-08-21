namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

public class Level2
{
    public Level2(int id)
    {
        Id = id;
    }

    public IList<Level3> Children { get; set; }
    public string Type { get; set; }
    public int ParentId { get; set; }
    public int Id { get; private set; }
}
