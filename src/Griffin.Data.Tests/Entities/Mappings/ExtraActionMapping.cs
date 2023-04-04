using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class ExtraActionMapping : IEntityConfigurator<ExtraAction>
{
    public void Configure(IClassMappingConfigurator<ExtraAction> config)
    {
        config.TableName("ExtraAction");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.ChildId);
        config.Property(x => x.Extra);
    }
}

internal class IActionMapping : IEntityConfigurator<IAction>
{
    public void Configure(IClassMappingConfigurator<IAction> config)
    {
        config.MapRemainingProperties();
    }
}