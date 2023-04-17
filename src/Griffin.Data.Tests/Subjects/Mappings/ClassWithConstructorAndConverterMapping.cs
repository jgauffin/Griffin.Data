using System.Data;
using Griffin.Data;
using Griffin.Data.Configuration;

public class ClassWithConstructorAndConverterMapping : IEntityConfigurator<ClassWithConstructorAndConverter>
{
    public void Configure(IClassMappingConfigurator<ClassWithConstructorAndConverter> config)
    {
        config.Property(x=>x.Name).Converter(new IntToStringConverter());
        config.MapRemainingProperties();
    }
}
