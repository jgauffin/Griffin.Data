using Griffin.Data;
using Griffin.Data.Configuration;
using DemoApp.Core.TodoTasks;

namespace DemoApp.Data.TodoTasks.Mappings
{
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
            config.HasOne(x => x.Data)
                .Discriminator(x => x.TaskType, SelectChildType)
                .ForeignKey(x => x.TaskId)
                .References(x => x.Id);

            config.MapRemainingProperties();
        }

        private static Type SelectChildType(int taskType)
        {
            return taskType switch
            {
                // Note that "TodoTask" from the table name
                // was automatically removed when the classes
                // where scaffolded.
                0 => typeof(DocumentReview),
                1 => typeof(GithubIssue),
                _ => throw new ArgumentOutOfRangeException(nameof(taskType), taskType, "Unknown task type.")
            };
        }
    }
}
