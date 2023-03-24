using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Converters.Enums;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Tests.Configuration.Subjects;

namespace Griffin.Data.Tests.Configuration;

public class PropertyConfigurationTests
{
    [Fact]
    public void Should_assign_column_name()
    {
        var mapping = new PropertyMapping(x => ((User)x).FirstName, (x, y) => ((User)x).FirstName = (string)y);
        var sut = new PropertyConfigurator<User, string>(mapping);

        sut.ColumnName("Name");

        mapping.ColumnName.Should().Be("Name");
    }

    [Fact]
    public void Should_assign_converter()
    {
        var mapping = new PropertyMapping(x => ((User)x).State, (x, y) => ((User)x).State = (AccountState)y);
        var sut = new PropertyConfigurator<User, AccountState>(mapping);

        sut.Converter(new ByteToEnum<AccountState>());

        mapping.PropertyToColumnConverter.Should().BeNull();
        mapping.ColumnToPropertyConverter.Should().BeNull();
    }

    [Fact]
    public void Should_assign_EnumToInt_as_default_converter_for_enums()
    {
        var mapping = new PropertyMapping(x => ((User)x).State, (x, y) => ((User)x).State = (AccountState)y);
        var sut = new PropertyConfigurator<User, AccountState>(mapping);
        var user = new User { State = AccountState.Admin };

        var a = mapping.GetColumnValue(user);
        mapping.SetColumnValue(user, 1);
        var b = mapping.GetColumnValue(user);

        a.Should().Be((int)AccountState.Admin);
        b.Should().Be((int)AccountState.Active);
    }
}