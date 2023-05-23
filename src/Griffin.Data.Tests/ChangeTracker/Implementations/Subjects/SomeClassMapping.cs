using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Subjects
{
    internal class SomeClassMapping : IEntityConfigurator<SomeClass>
    {
        public void Configure(IClassMappingConfigurator<SomeClass> config)
        {
            config.MapRemainingProperties();
        }
    }
}
