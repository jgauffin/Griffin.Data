using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class RootMapping : IEntityConfigurator<Level1>
{
    public void Configure(IClassMappingConfigurator<Level1> config)
    {
        config.Key(x => x.Id);
        config.HasOne(x => x.Child).ForeignKey(x => x.ParentId).References(x => x.Id);
        config.MapRemainingProperties();
    }
}
