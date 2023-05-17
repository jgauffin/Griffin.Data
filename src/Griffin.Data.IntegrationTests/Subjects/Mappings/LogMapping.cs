using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class LogMapping : IEntityConfigurator<Log>
{
    public void Configure(IClassMappingConfigurator<Log> config)
    {
        config.TableName("Logs");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.MainId);
        config.Property(x => x.CreatedAtUtc);
        config.Property(x => x.Message);
    }
}
