using Griffin.Data;
using Griffin.Data.Configuration;

namespace TestApp.Entities.Mappings;

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