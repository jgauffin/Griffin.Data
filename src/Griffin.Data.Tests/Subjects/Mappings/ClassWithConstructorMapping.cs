using System.Data;
using Griffin.Data;
using Griffin.Data.Configuration;

public class ClassWithConstructorMapping : IEntityConfigurator<ClassWithConstructor>
{
    public void Configure(IClassMappingConfigurator<ClassWithConstructor> config)
    {
        config.Property(x=>x.Name);
        config.MapRemainingProperties();
    }
}
