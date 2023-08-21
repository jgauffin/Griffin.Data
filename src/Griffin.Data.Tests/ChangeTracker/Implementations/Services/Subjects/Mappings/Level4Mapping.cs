using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class Level4Mapping : IEntityConfigurator<Level4>
{
    public void Configure(IClassMappingConfigurator<Level4> config)
    {
        config.Key(x => x.Id);
        config.MapRemainingProperties();
    }
}
