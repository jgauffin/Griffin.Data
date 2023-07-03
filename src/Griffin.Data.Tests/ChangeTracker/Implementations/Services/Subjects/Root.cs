namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

internal class Root
{
    public Root(int id)
    {
        Id = id;
    }

    public ChildWithChildren Child { get; set; }
    public string Name { get; set; }
    public int Id { get; private set; }
}
