using Griffin.Data;
using Griffin.Data.Configuration;
using DemoApp.Core.TodoTasks;

namespace DemoApp.Data.TodoTasks.Mappings
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
