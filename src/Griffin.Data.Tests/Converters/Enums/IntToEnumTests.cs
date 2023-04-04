using Griffin.Data.Converters.Enums;
using FluentAssertions;

namespace Griffin.Data.Tests.Converters.Enums
{
    public class IntToEnumTests
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

            var sut = new IntToEnum<B>();
            var actual = sut.PropertyToColumn(B.C);

            actual.Should().Be((byte)B.C);
        }

        [Fact]
        public void Should_be_able_to_convert_to_enum()
        {

            var sut = new IntToEnum<B>();
            var actual = sut.ColumnToProperty(1);

            actual.Should().Be(B.B);
        }
    }
}
