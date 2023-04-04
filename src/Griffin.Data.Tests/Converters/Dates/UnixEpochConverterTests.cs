using Griffin.Data.Converters.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Griffin.Data.Tests.Converters.Dates
{
    public class UnixEpochConverterTests
    {
        private DateTime Epoch = new DateTime(1970, 1, 1);

        [Fact]
        public void Should_convert_epoch_to_date()
        {
            var now = DateTime.UtcNow;
            
            var sut = new UnixEpochConverter();
            var d = sut.ColumnToProperty((long)now.Subtract(Epoch).TotalMilliseconds);

            d.Should().BeCloseTo(now, TimeSpan.FromMilliseconds(100));
        }


        [Fact]
        public void Should_convert_date_to_epoch()
        {
            var now = DateTime.UtcNow;
            var expected = (long)now.Subtract(Epoch).TotalMilliseconds;

            var sut = new UnixEpochConverter();
            var d = sut.PropertyToColumn(now);

            d.Should().Be(expected);
        }
    }
}
