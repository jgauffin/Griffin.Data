namespace TestApp.Entities;

public class ExtraAction: IAction
{
    public int Id { get; set; }

    public int ChildId { get; set; }

    public string Extra { get; set; } = "";
}