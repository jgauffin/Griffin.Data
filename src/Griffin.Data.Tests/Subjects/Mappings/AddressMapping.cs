using System.Globalization;
using Griffin.Data.Configuration;
using Griffin.Data.Converters;

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
        config.Property(x => x.State);
        config.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);

        config.MapRemainingProperties();
    }
}

public class DatesToStrings : ISingleValueConverter<string, DateTime>
{
    public DateTime ColumnToProperty(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    public string PropertyToColumn(DateTime value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}

internal class DataMapping : IEntityConfigurator<Data>
{
    public void Configure(IClassMappingConfigurator<Data> config)
    {
        config.MapRemainingProperties();
    }
}
