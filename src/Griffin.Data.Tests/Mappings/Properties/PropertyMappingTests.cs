using FluentAssertions;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Properties;
using Griffin.Data.Tests.Helpers;
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

    [Fact]
    public void Get_should_use_enum_converter_per_default_for_int()
    {
        AccountState state = AccountState.Admin;
        var sut = new PropertyMapping<User, AccountState>("State", user => user.State, (x, y) => state = y);

        var actual = sut.GetColumnValue(new User(){State = state });

        actual.Should().Be((int)state);
    }

    [Fact]
    public void Get_should_use_enum_converter_per_default_for_nullable_int()
    {
        AccountState? state = AccountState.Admin;
        var sut = new PropertyMapping<User, AccountState?>("State", user => user.NullableState, (x, y) => state = y);

        var actual = sut.GetColumnValue(new User() { NullableState = state });

        actual.Should().Be((int)state);
    }

    [Fact]
    public void Get_should_use_enum_converter_per_default_for_nullable_custom_primitive_type()
    {
        ExplicitState? state = ExplicitState.Admin;
        var sut = new PropertyMapping<User, ExplicitState?>("State", user => user.NullableExplicit, (x, y) => state = y);

        var actual = sut.GetColumnValue(new User() { NullableExplicit = state });

        actual.Should().Be((short)state);
    }

    [Fact]
    public void Convert_should_use_enum_converter_per_default()
    {
        AccountState state = AccountState.Admin;
        var sut = new PropertyMapping<User, AccountState>("State", user => user.State, (x, y) => state = y);

        var actual = sut.ConvertToColumnValue(state);

        actual.Should().Be((int)state);
    }


    [Fact]
    public void Set_should_use_enum_converter_per_default_for_string()
    {
        var record = new FakeRecord(new Dictionary<string, object>() { { "State", "2" } });
        AccountState state = AccountState.Disabled;
        var sut = new PropertyMapping<User, AccountState>("State", user => user.State, (x, y) => state = y);
        var entity = new User();

        sut.MapRecord(record, entity);

        state.Should().Be(AccountState.Admin);
    }

    [Fact]
    public void Set_should_use_enum_converter_per_default_for_int()
    {
        var record = new FakeRecord(new Dictionary<string, object>() { { "State", 2 } });
        AccountState state = AccountState.Disabled;
        var sut = new PropertyMapping<User, AccountState>("State", user => user.State, (x, y) => state = y);
        var entity = new User();

        sut.MapRecord(record, entity);

        state.Should().Be(AccountState.Admin);
    }

    [Fact]
    public void Set_should_use_enum_converter_per_default_for_short()
    {
        var record = new FakeRecord(new Dictionary<string, object>() { { "State", (short)2 } });
        ExplicitState state = ExplicitState.Disabled;
        var sut = new PropertyMapping<User, ExplicitState>("State", user => ExplicitState.Active, (x, y) => state = y);
        var entity = new User();
         
        sut.MapRecord(record, entity);

        state.Should().Be(ExplicitState.Admin);
    }

    [Fact]
    public void Set_should_use_enum_converter_per_default_for_short_from_property_to_record()
    {
        ExplicitState state = ExplicitState.Disabled;
        var sut = new PropertyMapping<User, ExplicitState>("State", user => ExplicitState.Active, (x, y) => state = y);

        var actual = sut.ConvertToColumnValue(ExplicitState.Active);

        actual.Should().Be(1);
    }

    [Fact]
    public void Set_should_explicit_enum_type()
    {
        var record = new FakeRecord(new Dictionary<string, object>() { { "State", (short)2 } });
        var state = ExplicitState.Disabled;
        var sut = new PropertyMapping<User, ExplicitState>("State", user => state, (x, y) => state = y);
        var entity = new User();

        sut.MapRecord(record, entity);

        state.Should().Be(ExplicitState.Admin);
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
