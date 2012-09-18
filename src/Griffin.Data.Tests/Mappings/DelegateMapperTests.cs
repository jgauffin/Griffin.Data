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
    public class DelegateMapperTests
    {
        [Fact]
        public void ThrowsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DelegateMapper<User>(null));
        }

        [Fact]
        public void Converts()
        {
            var mapper = new DelegateMapper<User>(ConvertRecord);

            var record = Substitute.For<IDataRecord>();
            var actual = mapper.Map(record);

            Assert.NotNull(actual);
        }

        private User ConvertRecord(IDataRecord arg)
        {
            return new User();
        }
    }
}
