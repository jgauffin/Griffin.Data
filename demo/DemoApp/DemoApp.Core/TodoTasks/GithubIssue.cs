
namespace DemoApp.Core.TodoTasks
{
    public class GithubIssue: ITaskData
    {
        public GithubIssue(int taskId, string name, int issueUrl)
        {
            TaskId = taskId;
            Name = name;
            IssueUrl = issueUrl;
        }

        public int TaskId { get; private set; }
        public string Name { get; private set; }
        public int IssueUrl { get; private set; }

    }
}
