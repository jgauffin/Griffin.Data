namespace TestApp.Entities;

public class ExtraAction : IAction
{
    public string Extra { get; set; } = "";
    public int Id { get; set; }

    public int ChildId { get; set; }
}
