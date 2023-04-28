using Griffin.Data.Domain;

namespace DemoApp.Domain.TodoTasks
{
    public interface IGithubIssueRepository : ICrudOperations<GithubIssue>
    {
        Task<GithubIssue> GetById(int taskId);
    }
}
