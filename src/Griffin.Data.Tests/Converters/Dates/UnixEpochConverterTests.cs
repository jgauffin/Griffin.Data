using FluentAssertions;
using Griffin.Data.Converters.Dates;

namespace Griffin.Data.Tests.Converters.Dates;

public class UnixEpochConverterTests
{
    private readonly DateTime Epoch = new(1970, 1, 1);

    [Fact]
    public void Should_convert_date_to_epoch()
    {
        var now = DateTime.UtcNow;
        var expected = (long)now.Subtract(Epoch).TotalMilliseconds;

        var sut = new UnixEpochConverter();
        var d = sut.PropertyToColumn(now);

        d.Should().Be(expected);
    }

    [Fact]
    public void Should_convert_epoch_to_date()
    {
        var now = DateTime.UtcNow;

        var sut = new UnixEpochConverter();
        var d = sut.ColumnToProperty((long)now.Subtract(Epoch).TotalMilliseconds);

        d.Should().BeCloseTo(now, TimeSpan.FromMilliseconds(100));
    }
}
