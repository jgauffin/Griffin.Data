using FluentAssertions;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.ChangeTracker.Implementations.Subjects;

namespace Griffin.Data.Tests.ChangeTracker.Implementations;

public class CopyConstructorFactoryTests
{
    [Fact]
    public void Should_be_able_to_create_class_with_parameter_constructor()
    {
        var sut = new CopyConstructorFactory();

        var factory = sut.CreateCopyConstructor(typeof(SomeClass));
        var source = new SomeClass(3);

        var actual = (SomeClass)factory(source);
        actual.Id.Should().Be(3);
    }

    [Fact]
    public void Should_be_able_to_create_class_with_default_constructor()
    {
        var sut = new CopyConstructorFactory();

        var factory = sut.CreateCopyConstructor(typeof(DefClass));
        var source = new DefClass();

        var actual = (DefClass)factory(source);
        actual.Should().NotBeNull();
    }

    [Fact]
    public void Should_report_if_constructor_arguments_do_not_match_properties()
    {
        var sut = new CopyConstructorFactory();

        Action actual = () => sut.CreateCopyConstructor(typeof(NotMatchingConstructor));

        actual.Should().Throw<MappingConfigurationException>().And.Message.Should().Contain("NotMatchingConstructor");
    }

}
