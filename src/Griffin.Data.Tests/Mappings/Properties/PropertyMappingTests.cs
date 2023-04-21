using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings.Properties;

public class PropertyMappingTests
{
    private User? _entity;
    private string? _value;

    [Fact]
    public void Convert_should_be_NoOp()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, null);

        var actual = sut.ConvertToColumnValue(3);

        actual.Should().Be(3);
    }

    [Fact]
    public void Should_be_able_to_get_value()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, SetValue);
        _value = "J";

        var actual = sut.GetColumnValue(new User());

        actual.Should().Be("J");
    }

    [Fact]
    public void Should_be_marked_as_non_readable_when_without_a_setter()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, null);

        sut.CanReadFromDatabase.Should().BeFalse();
    }

    [Fact]
    public void Should_be_marked_as_non_writable_when_without_a_getter()
    {
        var sut = new PropertyMapping<User, string>("FirstName", null, SetValue);

        sut.CanWriteToDatabase.Should().BeFalse();
    }

    [Fact]
    public void Should_be_marked_as_readable_when_having_a_setter()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, SetValue);

        sut.CanReadFromDatabase.Should().BeTrue();
    }

    [Fact]
    public void Should_be_marked_as_writable_when_having_a_getter()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, SetValue);

        sut.CanWriteToDatabase.Should().BeTrue();
    }

    [Fact]
    public void Should_throw_when_no_getter_is_specified()
    {
        var sut = new PropertyMapping<User, string>("FirstName", null, SetValue);

        var actual = () => sut.GetColumnValue("");

        actual.Should().Throw<MappingException>();
    }

    [Fact]
    public void Should_use_write_converter_when_specified()
    {
        var sut = new PropertyMapping<User, string>("FirstName", GetValue, SetValue)
        {
            PropertyToColumnConverter = x => int.Parse(x)
        };
        _value = "5";

        var actual = sut.GetColumnValue(new User());

        actual.Should().Be(5);
    }

    private string GetValue(User arg)
    {
        return _value ?? "";
    }

    private void SetValue(User arg1, string arg2)
    {
        _value = arg2;
        _entity = arg1;
    }
}
