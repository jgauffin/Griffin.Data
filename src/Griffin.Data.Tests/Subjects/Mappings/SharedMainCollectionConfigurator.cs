using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

internal class SharedMainCollectionConfigurator : IEntityConfigurator<SharedMainCollection>
{
    public void Configure(IClassMappingConfigurator<SharedMainCollection> config)
    {
        config.TableName("SharedMain");
        config.Key(x => x.Id).AutoIncrement();
        config.HasMany(x => x.Left)
            .SubsetColumn("ParentProperty", "Left")
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);
        config.HasMany(x => x.Right)
            .SubsetColumn("ParentProperty", "Right")
            .ForeignKey(x => x.MainId)
            .References(x => x.Id);
    }
}