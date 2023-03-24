using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entitites.Mappings;

internal class LogMapping : IEntityConfigurator<Log>
{
    public void Configure(IClassMappingConfigurator<Log> config)
    {
        config.TableName("Logs");
        config.Property(x => x.MainId);
        config.Property(x => x.CreatedAtUtc);
        config.Property(x => x.Message);
    }
}