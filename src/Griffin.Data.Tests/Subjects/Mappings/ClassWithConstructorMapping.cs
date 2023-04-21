using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings;

public class ClassWithConstructorMapping : IEntityConfigurator<ClassWithConstructor>
{
    public void Configure(IClassMappingConfigurator<ClassWithConstructor> config)
    {
        config.Property(x => x.Name);
        config.MapRemainingProperties();
    }
}
