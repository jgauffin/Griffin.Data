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
    public class ByteToEnumTests
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

            var sut = new ByteToEnum<B>();
            var actual  =sut.PropertyToColumn(B.C);

            actual.Should().Be((byte)B.C);
        }

        [Fact]
        public void Should_be_able_to_convert_to_enum()
        {

            var sut = new ByteToEnum<B>();
            var actual = sut.ColumnToProperty(1);

            actual.Should().Be(B.B);
        }
    }
}
