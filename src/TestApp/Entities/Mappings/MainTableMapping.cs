using Griffin.Data;
using Griffin.Data.Configuration;

namespace TestApp.Entities.Mappings;

internal class MainTableMapping : IEntityConfigurator<MainTable>
{
    public void Configure(IClassMappingConfigurator<MainTable> config)
    {
        config.TableName("MainTable");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.Name);
        config.Property(x => x.Age);
        config.Property(x => x.Money);
        config.Property(x => x.Rocks);

        config.HasMany(x => x.Children)
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);

        config.HasMany(x => x.Logs)
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);
    }
}
