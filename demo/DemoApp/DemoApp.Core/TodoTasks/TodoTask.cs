namespace DemoApp.Core.TodoTasks;

public class TodoTask
{
    private readonly List<DocumentReview> _documentReviews = new();
    private readonly List<GithubIssue> _githubIssues = new();

    public TodoTask(
        int todolistId,
        string name,
        int taskType,
        TodoTaskState state,
        int priority,
        int createdById,
        DateTime createdAtUtc)
    {
        TodolistId = todolistId;
        Name = name;
        TaskType = taskType;
        State = state;
        Priority = priority;
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }

    public DateTime CreatedAtUtc { get; }

    public int CreatedById { get; }
    // public GithubIssue GithubIssue { get; set; }

    public IReadOnlyList<DocumentReview> DocumentReviews => _documentReviews;

    public IReadOnlyList<GithubIssue> GithubIssues => _githubIssues;

    public int Id { get; private set; }
    public string Name { get; }
    public int Priority { get; }
    public TodoTaskState State { get; }
    public int TaskType { get; }
    public int TodolistId { get; }
    public DateTime UpdatedAtUtc { get; set; }

    public int UpdatedById { get; set; }
    // public DocumentReview DocumentReview { get; set; }
}
