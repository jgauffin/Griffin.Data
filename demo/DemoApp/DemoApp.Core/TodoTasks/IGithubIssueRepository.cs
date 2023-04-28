namespace DemoApp.Core.TodoTasks;

public interface IGithubIssueRepository
{
    Task Create(GithubIssue entity);

    Task Delete(GithubIssue entity);
    Task<GithubIssue> GetById(int taskId);

    Task Update(GithubIssue entity);
}
