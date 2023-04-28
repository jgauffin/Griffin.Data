using Griffin.Data;
using Griffin.Data.Configuration;
using D:\src\jgauffin\Griffin.Data\src\DemoApp\Domain.Todolists;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data.Todolists.Mappings
{
    public class TodolistMapping : IEntityConfigurator<Todolist>
    {
        public void Configure(IClassMappingConfigurator<Todolist> config)
        {
            config.TableName("Todolists");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.Name);
            config.Property(x => x.CreatedById);
            config.Property(x => x.CreatedAtUtc);
            config.Property(x => x.UpdatedById);
            config.Property(x => x.UpdatedAtUtc);
            config.HasMany(x => x.Permissions)
                  .ForeignKey(x => x.TodolistId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.Permission)
            //      .ForeignKey(x => x.TodolistId)
            //      .References(x => x.Id);

            config.HasMany(x => x.TodoTasks)
                  .ForeignKey(x => x.TodolistId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.TodoTask)
            //      .ForeignKey(x => x.TodolistId)
            //      .References(x => x.Id);

        }
    }
}
