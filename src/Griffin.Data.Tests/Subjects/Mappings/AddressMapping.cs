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
