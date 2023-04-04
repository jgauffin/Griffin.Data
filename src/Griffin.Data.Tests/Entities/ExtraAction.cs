namespace Griffin.Data.Tests.Entities;

public class ExtraAction : IAction
{
    public int Id { get; set; }

    public string Extra { get; set; }
    public int ChildId { get; set; }
}