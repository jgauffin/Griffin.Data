namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

public class ChildWithChildren
{
    public ChildWithChildren(int id)
    {
        Id = id;
    }

    public IList<Child> Children { get; set; }
    public string Type { get; set; }
    public int ParentId { get; set; }
    public int Id { get; private set; }
}
