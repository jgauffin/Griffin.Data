using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data.Converters.Enums;

namespace Griffin.Data.Tests.Converters.Enums
{
    public class StringToEnumTests
    {
        enum B
        {
            A,
            B, 
            C
        }

        [Fact]
        public void Should_be_able_to_convert_to_byte()
        {

            var sut = new StringToEnum<B>();
            var actual  =sut.PropertyToColumn(B.C);

            actual.Should().Be("C");
        }

        [Fact]
        public void Should_be_able_to_convert_to_enum()
        {

            var sut = new StringToEnum<B>();
            var actual = sut.ColumnToProperty("C");

            actual.Should().Be(B.C);
        }
    }
}
