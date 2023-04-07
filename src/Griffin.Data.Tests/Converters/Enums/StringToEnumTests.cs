using FluentAssertions;
using Griffin.Data.Converters.Enums;

namespace Griffin.Data.Tests.Converters.Enums;

public class StringToEnumTests
{
    [Fact]
    public void Should_be_able_to_convert_to_byte()
    {
        var sut = new StringToEnum<B>();
        var actual = sut.PropertyToColumn(B.C);

        actual.Should().Be("C");
    }

    [Fact]
    public void Should_be_able_to_convert_to_enum()
    {
        var sut = new StringToEnum<B>();
        var actual = sut.ColumnToProperty("C");

        actual.Should().Be(B.C);
    }

    private enum B
    {
        A,
        B,
        C
    }
}
