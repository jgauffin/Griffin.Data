using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entitites.Mappings;

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
        config.Property(x => x.ActionType);
        config.HasOne(x => x.ExtraAction)
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);

        config.HasMany(x => x.Logs)
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);
    }
}