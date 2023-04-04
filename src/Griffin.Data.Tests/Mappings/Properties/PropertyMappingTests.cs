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
    public class PropertyMappingTests
    {
        private object? _value;
        private object _entity;

        [Fact]
        public void Should_be_able_to_set_value()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue);

            sut.SetColumnValue("", 1);

            _entity.Should().Be("");
            _value.Should().Be(1);
        }

        [Fact]
        public void Should_throw_when_no_setter_is_specified()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, null);

            var actual = () => sut.SetColumnValue("", 1);

            actual.Should().Throw<MappingException>();
        }

        [Fact]
        public void Should_be_able_to_get_value()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue);
            _value = 3;

            var actual = sut.GetColumnValue("");

            actual.Should().Be(3);
        }

        [Fact]
        public void Should_throw_when_no_getter_is_specified()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), null, SetValue);

            var actual = () => sut.GetColumnValue("");

            actual.Should().Throw<MappingException>();
        }

        [Fact]
        public void Convert_should_be_NoOp()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, null);

            var actual = sut.ToColumnValue(3);

            actual.Should().Be(3);
        }

        [Fact]
        public void Should_be_marked_as_writable_when_having_a_getter()
        {

            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue);

            sut.CanWriteToDatabase.Should().BeTrue();
        }

        [Fact]
        public void Should_be_marked_as_non_writable_when_without_a_getter()
        {

            var sut = new PropertyMapping(typeof(User), typeof(string), null, SetValue);

            sut.CanWriteToDatabase.Should().BeFalse();
        }

        [Fact]
        public void Should_be_marked_as_readable_when_having_a_setter()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue);

            sut.CanReadFromDatabase.Should().BeTrue();
        }

        [Fact]
        public void Should_be_marked_as_non_readable_when_without_a_setter()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, null);

            sut.CanReadFromDatabase.Should().BeFalse();
        }

        [Fact]
        public void Should_use_write_converter_when_specified()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue)
            {
                PropertyToColumnConverter = x => int.Parse((string)x)
            };
            _value = "5";

            var actual = sut.GetColumnValue("");

            actual.Should().Be(5);
        }

        [Fact]
        public void Should_use_read_converter_when_specified()
        {
            var sut = new PropertyMapping(typeof(User), typeof(string), GetValue, SetValue)
            {
                ColumnToPropertyConverter = x => int.Parse((string)x)
            };

            sut.SetColumnValue("", "5");

            _value.Should().Be(5);
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
