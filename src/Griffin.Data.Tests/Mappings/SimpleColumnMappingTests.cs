using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mappings;
using NSubstitute;
using Xunit;

namespace Griffin.Data.Tests.Mappings
{
    public class SimpleColumnMappingTests
    {
        [Fact]
        public void NoConverter()
        {
            var mapping = new SimpleColumnMapping<User>(x => x.FirstName, "first_name", null);
            var dataRecord = Substitute.For<IDataRecord>();
            dataRecord["first_name"].Returns("arne");
            var actual = new User();

            mapping.SetValue(dataRecord, actual);

            Assert.Equal("arne", actual.FirstName);
        }

        [Fact]
        public void CallingConverter()
        {
            var converter = Substitute.For<IColumnConverter>();
            converter.ConvertFromDb("arne").Returns("Arne");
            var mapping = new SimpleColumnMapping<User>(x => x.FirstName, "first_name", converter);
            var dataRecord = Substitute.For<IDataRecord>();
            dataRecord["first_name"].Returns("arne");
            var actual = new User();

            mapping.SetValue(dataRecord, actual);

            Assert.Equal("Arne", actual.FirstName);
            converter.Received().ConvertFromDb("arne");
        }
    }
}
