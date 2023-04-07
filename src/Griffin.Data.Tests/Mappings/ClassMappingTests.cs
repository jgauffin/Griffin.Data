using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings;

public class ClassMappingTests
{
    private readonly ClassMapping _mapping;

    public ClassMappingTests()
    {
        var userConfig = new ClassMappingConfigurator<User>();
        userConfig.Key(x => x.Id);
        userConfig.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);
        userConfig.Property(x => x.FirstName).ColumnName("first_name");
        userConfig.MapRemainingProperties();
        _mapping = ((IMappingBuilder)userConfig).BuildMapping();
    }

    [Fact]
    public void Should_find_an_existing_key()
    {
        var prop = _mapping.FindPropertyByName("id")!;

        var actual = prop.GetColumnValue(new User { Id = 3 });

        actual.Should().Be(3);
    }

    [Fact]
    public void Should_find_an_existing_property()
    {
        var prop = _mapping.FindPropertyByName("FirstName")!;

        var actual = prop.GetColumnValue(new User { FirstName = "J" });

        actual.Should().Be("J");
    }

    [Fact]
    public void Should_find_an_existing_property_by_column_name()
    {
        var prop = _mapping.FindPropertyByName("first_name")!;

        var actual = prop.GetColumnValue(new User { FirstName = "J" });

        actual.Should().Be("J");
    }

    [Fact]
    public void Should_get_an_existing_key()
    {
        var prop = _mapping.GetProperty("Id");

        var actual = prop.GetColumnValue(new User { Id = 3 });

        actual.Should().Be(3);
    }

    [Fact]
    public void Should_get_an_existing_property()
    {
        var prop = _mapping.GetProperty("FirstName");

        var actual = prop.GetColumnValue(new User { FirstName = "J" });

        actual.Should().Be("J");
    }

    [Fact]
    public void Should_throw_if_property_is_not_found()
    {
        var actual = () => _mapping.GetProperty("SomeProperty");

        actual.Should().Throw<MappingException>();
    }
}
