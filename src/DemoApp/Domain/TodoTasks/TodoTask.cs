
namespace DemoApp.Domain.TodoTasks
{
    public class TodoTask
    {
        private readonly List<GithubIssue> _githubIssues = new();
        private readonly List<DocumentReview> _documentReviews = new();

        public TodoTask(int todolistId, string name, TaskType taskType, TodoTaskState state, int priority, int createdById)
        {
            TodolistId = todolistId;
            Name = name;
            TaskType = taskType;
            State = state;
            Priority = priority;
            CreatedById = createdById;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public int TodolistId { get; private set; }
        public string Name { get; private set; }
        public int TaskType { get; private set; }
        public TodoTaskState State { get; private set; }
        public int Priority { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int UpdatedById { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public ITaskData Data { get; set; }

    }
}
