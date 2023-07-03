using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects.Mappings;

internal class ChildMapping : IEntityConfigurator<Child>
{
    public void Configure(IClassMappingConfigurator<Child> config)
    {
        config.Key(x => x.Id);
        config.MapRemainingProperties();
    }
}
