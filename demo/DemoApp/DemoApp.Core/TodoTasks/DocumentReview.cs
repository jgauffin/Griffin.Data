namespace DemoApp.Core.TodoTasks;

public class DocumentReview
{
    public DocumentReview(int taskId, string documentUrl)
    {
        TaskId = taskId;
        DocumentUrl = documentUrl;
    }

    public string Comment { get; set; }
    public string DocumentUrl { get; }

    public int TaskId { get; }
}
