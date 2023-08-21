using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class ChildWithChildrenMapping : IEntityConfigurator<Level2>
{
    public void Configure(IClassMappingConfigurator<Level2> config)
    {
        config.Key(x => x.Id);
        config.HasMany(x => x.Children).ForeignKey(x => x.ParentId).References(x => x.Id);
        config.MapRemainingProperties();
    }
}
