using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class ChildWithChildrenMapping : IEntityConfigurator<ChildWithChildren>
{
    public void Configure(IClassMappingConfigurator<ChildWithChildren> config)
    {
        config.Key(x => x.Id);
        config.HasMany(x => x.Children).ForeignKey(x => x.ParentId).References(x => x.Id);
        config.MapRemainingProperties();
    }
}
