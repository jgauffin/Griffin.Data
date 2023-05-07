using DemoApp.Core.TodoTasks;
using Griffin.Data;
using Griffin.Data.Configuration;

namespace DemoApp.Data.TodoTasks.Mappings
{
    internal class TaskDataMapping : IEntityConfigurator<ITaskData>
    {
        public void Configure(IClassMappingConfigurator<ITaskData> config)
        {
            config.Key(x => x.TaskId);
            config.MapRemainingProperties();
        }
    }
}
