using Griffin.Data;
using Griffin.Data.Configuration;

namespace TestApp.Entities.Mappings;

internal class IActionMapping : IEntityConfigurator<IAction>
{
    public void Configure(IClassMappingConfigurator<IAction> config)
    {
        config.Property(x => x.ChildId);
    }
}
