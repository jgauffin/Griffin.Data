namespace Griffin.Data.Tests.Entities
{
    public class ChildTable
    {
        public int Id { get; set; }
        public int MainId { get; set; }
        public ActionType ActionType { get; set; }
        public IAction Action { get; set; }
    }
}
