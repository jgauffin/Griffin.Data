using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class MainTableMapping2 : IEntityConfigurator<MainTable2>
{
    public void Configure(IClassMappingConfigurator<MainTable2> config)
    {
        config.TableName("MainTable");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.Name);
        config.Property(x => x.Age);
        config.Property(x => x.Money).Converter(BigIntUlong.Instance);
        config.Property(x => x.Rocks);
    }
}