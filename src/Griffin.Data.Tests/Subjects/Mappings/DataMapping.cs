using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

internal class DataMapping : IEntityConfigurator<Data>
{
    public void Configure(IClassMappingConfigurator<Data> config)
    {
        config.MapRemainingProperties();
    }
}