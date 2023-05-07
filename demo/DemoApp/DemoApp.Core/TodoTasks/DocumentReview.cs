
namespace DemoApp.Core.TodoTasks
{
    public class DocumentReview : ITaskData
    {
        public DocumentReview(int taskId, string documentUrl)
        {
            TaskId = taskId;
            DocumentUrl = documentUrl;
        }

        public int TaskId { get; private set; }
        public string DocumentUrl { get; private set; }
        public string? Comment { get; set; }

    }
}
