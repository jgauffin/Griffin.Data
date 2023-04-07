using Griffin.Data;
using Griffin.Data.Configuration;

namespace TestApp.Entities.Mappings;

internal class ChildTableMapping : IEntityConfigurator<ChildTable>
{
    public void Configure(IClassMappingConfigurator<ChildTable> config)
    {
        config.TableName("ChildTable");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.MainId);

        config.HasOne(x => x.Action)
            .Discriminator(x => x.ActionType, ChildSelector)
            .ForeignKey(x => x.ChildId)
            .References(x => x.Id);

        config.MapRemainingProperties();
    }

    private Type? ChildSelector(ActionType arg)
    {
        return arg switch
        {
            ActionType.Simple => typeof(SimpleAction),
            ActionType.Extra => typeof(ExtraAction),
            _ => null
        };
    }
}
