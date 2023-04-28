using DemoApp.Core.TodoTasks;
using Griffin.Data;
using Griffin.Data.Domain;
using Griffin.Data.Mapper;

namespace DemoApp.Data.TodoTasks;

public class GithubIssueRepository : CrudOperations<GithubIssue>, IGithubIssueRepository
{
    public GithubIssueRepository(Session session) : base(session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }
    }

    public async Task<GithubIssue> GetById(int taskId)
    {
        return await Session.First<GithubIssue>(new { taskId });
    }
}
