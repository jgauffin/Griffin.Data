using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.TableName("Users");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.State);
        config.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);

        config.MapRemainingProperties();
    }
}
