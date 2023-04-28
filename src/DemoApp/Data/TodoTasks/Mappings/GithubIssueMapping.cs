using Griffin.Data;
using Griffin.Data.Configuration;
using D:\src\jgauffin\Griffin.Data\src\DemoApp\Domain.TodoTasks;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data.TodoTasks.Mappings
{
    public class GithubIssueMapping : IEntityConfigurator<GithubIssue>
    {
        public void Configure(IClassMappingConfigurator<GithubIssue> config)
        {
            config.TableName("TodoTaskGithubIssues");
            config.Key(x => x.TaskId).AutoIncrement();
            config.Property(x => x.Name);
            config.Property(x => x.IssueUrl);
        }
    }
}
