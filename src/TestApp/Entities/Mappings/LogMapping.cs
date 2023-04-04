using Griffin.Data;
using Griffin.Data.Configuration;

namespace TestApp.Entities.Mappings;

internal class LogMapping : IEntityConfigurator<Log>
{
    public void Configure(IClassMappingConfigurator<Log> config)
    {
        config.TableName("Logs");
        config.Key(x => x.Id);
        config.Property(x => x.MainId);
        config.Property(x => x.CreatedAtUtc);
        config.Property(x => x.Message);
    }
}