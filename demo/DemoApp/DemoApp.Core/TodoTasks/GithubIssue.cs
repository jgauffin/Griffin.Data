namespace DemoApp.Core.TodoTasks;

public class GithubIssue
{
    public GithubIssue(int taskId, string name, int issueUrl)
    {
        TaskId = taskId;
        Name = name;
        IssueUrl = issueUrl;
    }

    public int IssueUrl { get; }
    public string Name { get; }

    public int TaskId { get; }
}
