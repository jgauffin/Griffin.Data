using Griffin.Data.Configuration;
using Griffin.Data.Tests.Helpers;

namespace Griffin.Data.Tests.Subjects.Mappings;

public class ClassWithConstructorAndConverterMapping : IEntityConfigurator<ClassWithConstructorAndConverter>
{
    public void Configure(IClassMappingConfigurator<ClassWithConstructorAndConverter> config)
    {
        config.Property(x => x.Name).Converter(new IntToStringConverter());
        config.MapRemainingProperties();
    }
}
