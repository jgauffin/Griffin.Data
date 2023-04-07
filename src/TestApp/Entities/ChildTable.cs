namespace TestApp.Entities;

public class ChildTable
{
    public ChildTable(ActionType actionType, IAction action)
    {
        ActionType = actionType;
        Action = action;
    }

    public IAction Action { get; set; }
    public ActionType ActionType { get; set; }

    public int Id { get; set; }
    public int MainId { get; set; }
}
