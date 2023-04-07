using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Entities.Mappings;

internal class ChildTableMapping : IEntityConfigurator<ChildTable>
{
    public void Configure(IClassMappingConfigurator<ChildTable> config)
    {
        config.TableName("ChildTable");
        config.Key(x => x.Id).AutoIncrement();
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
            ActionType.Created => typeof(ExtraAction),
            ActionType.Active => typeof(SimpleAction),
            _ => null
        };
    }
}
