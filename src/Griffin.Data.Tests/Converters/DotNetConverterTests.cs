using FluentAssertions;
using Griffin.Data.Converters;

namespace Griffin.Data.Tests.Converters;

public class DotNetConverterTests
{
    [Fact]
    public void Should_be_able_to_convert_column_value()
    {
        var sut = new DotNetConverter<int, long>();
        var actual = sut.ColumnToProperty(5);

        actual.Should().Be(5);
        actual.Should().BeOfType(typeof(long));
    }

    [Fact]
    public void Should_be_able_to_convert_property_value()
    {
        var sut = new DotNetConverter<int, long>();
        var actual = sut.PropertyToColumn(5L);

        actual.Should().Be(5);
        actual.Should().BeOfType(typeof(int));
    }
}
