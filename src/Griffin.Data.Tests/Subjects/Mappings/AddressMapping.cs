using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

internal class AddressMapping : IEntityConfigurator<Address>
{
    public void Configure(IClassMappingConfigurator<Address> config)
    {
        config.TableName("Addresses");
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemainingProperties();
    }
}

internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.TableName("Users");
        config.Key(x => x.Id).AutoIncrement();
        config.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);

        config.MapRemainingProperties();
    }
}

internal class DataMapping : IEntityConfigurator<Data>
{
    public void Configure(IClassMappingConfigurator<Data> config)
    {
        config.MapRemainingProperties();
    }
}
