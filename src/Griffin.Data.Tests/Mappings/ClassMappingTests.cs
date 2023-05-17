using System.Reflection;
using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Tests.Helpers;
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
        _mapping = ((IMappingBuilder)userConfig).BuildMapping(true);
    }

    [Fact]
    public void Should_be_able_to_create_instance_using_default_constructor()
    {
        var record = new FakeRecord(new Dictionary<string, object>());

        var actual = _mapping.CreateInstance(record);

        actual.Should().NotBeNull();
    }

    [Fact]
    public void Should_be_able_to_create_instance_using_non_default_constructor()
    {
        var reg = new MappingRegistry();
        reg.Scan(Assembly.GetExecutingAssembly());
        var mapping = reg.Get<ClassWithConstructor>();
        var record = new FakeRecord(new Dictionary<string, object> { { "Id", 1 }, { "Name", "Jonas" } });

        var actual = (ClassWithConstructor)mapping.CreateInstance(record);

        actual.Id.Should().Be(1);
        actual.Name.Should().Be("Jonas");
    }

    [Fact]
    public void Should_be_able_to_create_instance_using_non_default_constructor_and_converter()
    {
        var reg = new MappingRegistry();
        reg.Scan(Assembly.GetExecutingAssembly());
        var mapping = reg.Get<ClassWithConstructorAndConverter>();
        var record = new FakeRecord(new Dictionary<string, object> { { "Id", 1 }, { "Name", 3 } });

        var actual = (ClassWithConstructorAndConverter)mapping.CreateInstance(record);

        actual.Id.Should().Be(1);
        actual.Name.Should().Be("3");
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

    [Fact]
    public void MapRemainingProperties_should_map_enum()
    {
        var entity = new User() { State = AccountState.Admin };

        var prop = _mapping.GetProperty("State");
        var actual = prop.GetColumnValue(entity);

        actual.Should().Be(2);
    }
}
