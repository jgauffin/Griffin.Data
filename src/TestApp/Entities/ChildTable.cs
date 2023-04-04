namespace TestApp.Entities
{
    public class ChildTable
    {
        public ChildTable(ActionType actionType, IAction action)
        {
            ActionType = actionType;
            Action = action;
        }

        public int Id { get; set; }
        public int MainId { get; set; }
        public ActionType ActionType { get; set; }
        public IAction Action { get; set; }
    }
}
