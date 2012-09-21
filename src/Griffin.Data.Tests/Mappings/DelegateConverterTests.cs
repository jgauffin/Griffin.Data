using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Converters;
using Griffin.Data.Mappings;
using Xunit;

namespace Griffin.Data.Tests.Mappings
{
    public class DelegateConverterTests
    {
        [Fact]
        public void ThrowsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DelegateConverter<int, string>(null));
        }

        [Fact]
        public void Converts()
        {
            var converter = new DelegateConverter<int, string>(i => i.ToString());

            var actual = converter.Convert(10);

            Assert.Equal("10", actual);
        }
    }
}
