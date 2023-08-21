using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class Level3Mapping : IEntityConfigurator<Level3>
{
    public void Configure(IClassMappingConfigurator<Level3> config)
    {
        config.Key(x => x.Id);
        config.HasOne(x => x.Child4).ForeignKey(x => x.ParentId).References(x => x.Id);
        config.MapRemainingProperties();
    }
}
