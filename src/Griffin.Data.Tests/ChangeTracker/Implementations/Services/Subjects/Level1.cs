namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

internal class Level1
{
    public Level1(int id)
    {
        Id = id;
    }

    public Level2 Child { get; set; }
    public string Name { get; set; }
    public int Id { get; private set; }
}
