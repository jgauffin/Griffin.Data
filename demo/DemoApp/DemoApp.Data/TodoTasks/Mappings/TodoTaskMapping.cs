using DemoApp.Core.TodoTasks;
using Griffin.Data;
using Griffin.Data.Configuration;

namespace DemoApp.Data.TodoTasks.Mappings;

public class TodoTaskMapping : IEntityConfigurator<TodoTask>
{
    public void Configure(IClassMappingConfigurator<TodoTask> config)
    {
        config.TableName("TodoTasks");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.TodolistId);
        config.Property(x => x.Name);
        config.Property(x => x.TaskType);
        config.Property(x => x.State);
        config.Property(x => x.Priority);
        config.Property(x => x.CreatedById);
        config.Property(x => x.CreatedAtUtc);
        config.Property(x => x.UpdatedById);
        config.Property(x => x.UpdatedAtUtc);
        config.HasMany(x => x.GithubIssues)
            .ForeignKey(x => x.TaskId)
            .References(x => x.Id);
        //config.HasOneConfigurator(x => x.GithubIssue)
        //      .ForeignKey(x => x.TaskId)
        //      .References(x => x.Id);

        config.HasMany(x => x.DocumentReviews)
            .ForeignKey(x => x.TaskId)
            .References(x => x.Id);
        //config.HasOneConfigurator(x => x.DocumentReview)
        //      .ForeignKey(x => x.TaskId)
        //      .References(x => x.Id);
    }
}
