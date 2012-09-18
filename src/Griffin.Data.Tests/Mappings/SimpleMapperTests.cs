using System.Data;
using Griffin.Data.Mappings;
using NSubstitute;
using Xunit;
using IColumnMapping = Griffin.Data.Mappings.IColumnMapping;

namespace Griffin.Data.Tests.Mappings
{
    public class SimpleMapperTests
    {
        [Fact]
        public void InvokeMappedOnly()
        {
            var mapper = new SimpleMapper<User>();
            mapper.Add(x => x.FirstName, "first_name");
            var customMapping = Substitute.For<IColumnMapping>();
            customMapping.SetValue(Arg.Any<IDataRecord>(), Arg.Any<object>());
            mapper.Add(customMapping);
            var record = Substitute.For<IDataRecord>();
            record["first_name"].Returns("Jonas");
            record["age"].Returns(10);

            var actual = mapper.Map(record);

            Assert.Equal("Jonas", actual.FirstName);
            Assert.Equal(0, actual.Id);
            customMapping.Received().SetValue(Arg.Any<IDataRecord>(), Arg.Any<object>());
        }

    }
}
