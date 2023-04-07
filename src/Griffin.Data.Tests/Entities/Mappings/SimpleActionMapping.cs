using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class SimpleActionMapping : IEntityConfigurator<SimpleAction>
{
    public void Configure(IClassMappingConfigurator<SimpleAction> config)
    {
        config.TableName("SimpleAction");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.ChildId);
        config.Property(x => x.Simple);
    }
}
