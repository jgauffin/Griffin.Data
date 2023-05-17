using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

internal class SharedChildConfigurator : IEntityConfigurator<SharedChild>
{
    public void Configure(IClassMappingConfigurator<SharedChild> config)
    {
        config.TableName("SharedChild");
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemainingProperties();
    }
}