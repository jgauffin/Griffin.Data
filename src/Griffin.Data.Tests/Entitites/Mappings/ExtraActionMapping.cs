using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entitites.Mappings;

internal class ExtraActionMapping : IEntityConfigurator<ExtraAction>
{
    public void Configure(IClassMappingConfigurator<ExtraAction> config)
    {
        config.TableName("ExtraAction");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.MainId);
        config.Property(x => x.Extra);
    }
}