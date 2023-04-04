namespace TestApp.Entities;

public class SimpleAction : IAction
{
    public int Id { get; set; }

    public int ChildId { get; set; }

    public int Simple { get; set; }
}