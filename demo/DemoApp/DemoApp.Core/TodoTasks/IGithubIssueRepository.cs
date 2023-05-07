using DemoApp.Core;

namespace DemoApp.Core.TodoTasks
{
    public interface IGithubIssueRepository
    {
        Task<GithubIssue> GetById(int taskId);

        Task Create(GithubIssue entity);

        Task Update(GithubIssue entity);

        Task Delete(GithubIssue entity);

    }
}
