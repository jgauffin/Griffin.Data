using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings.Properties
{
    public class KeyMappingTests
    {
        private object? _value;
        private object _entity;

        [Fact]
        public void Should_be_able_to_set_value()
        {
            var sut = new KeyMapping(typeof(User), GetValue, SetValue);

            sut.SetColumnValue("", 1);

            _entity.Should().Be("");
            _value.Should().Be(1);
        }

        [Fact]
        public void Should_throw_when_no_setter_is_specified()
        {
            var sut = new KeyMapping(typeof(User), GetValue, null);

            var actual = () => sut.SetColumnValue("", 1);

            actual.Should().Throw<MappingException>();
        }

        [Fact]
        public void Should_be_able_to_get_value()
        {
            var sut = new KeyMapping(typeof(User), GetValue, SetValue);
            _value = 3;

            var actual = sut.GetColumnValue("");

            actual.Should().Be(3);
        }

        [Fact]
        public void Should_throw_when_no_getter_is_specified()
        {
            var sut = new KeyMapping(typeof(User), null, SetValue);

            var actual = () => sut.GetColumnValue("");

            actual.Should().Throw<MappingException>();
        }

        [Fact]
        public void Convert_should_be_NoOp()
        {
            var sut = new KeyMapping(typeof(User), GetValue, null);

            var actual = sut.ToColumnValue(3);

            actual.Should().Be(3);
        }

        private void SetValue(object arg1, object arg2)
        {
            _value = arg2;
            _entity = arg1;
        }

        private object? GetValue(object arg)
        {
            return _value;
        }
    }
}
