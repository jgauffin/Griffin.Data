namespace Griffin.Data.Tests.Entities;

public class ChildTable
{
    public IAction Action { get; set; }
    public ActionType ActionType { get; set; }
    public int Id { get; set; }
    public int MainId { get; set; }
}
