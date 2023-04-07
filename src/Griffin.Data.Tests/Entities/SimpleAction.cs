namespace Griffin.Data.Tests.Entities;

public class SimpleAction : IAction
{
    public int Id { get; set; }

    public int Simple { get; set; }

    public int ChildId { get; set; }
}
