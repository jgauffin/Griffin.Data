using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mappings;
using Xunit;

namespace Griffin.Data.Tests.Mappings
{
    public class MapperProviderTests
    {
        [Fact]
        public void DuplicateMappings()
        {
            var provider = new MapperProvider();
            
            provider.RegisterAssembly(Assembly.GetExecutingAssembly());
            
            
            Assert.Throws<MappingException>(() => provider.Register(new CustomMapper()));
        }

        [Fact]
        public void SingleMapping()
        {
            var provider = new MapperProvider();
            
            provider.RegisterAssembly(Assembly.GetExecutingAssembly());

            Assert.NotNull(provider.GetMapper<MapperProviderTests>());
        }

        [Fact]
        public void NoMapping()
        {
            var provider = new MapperProvider();

            provider.RegisterAssembly(Assembly.GetExecutingAssembly());

            Assert.Throws<ArgumentOutOfRangeException>(() => provider.GetMapper<string>());   
        }

    }

    public class SingleMapping : SimpleMapper<MapperProviderTests>
    {
        /// <summary>
        /// Map a record to a new entity
        /// </summary>
        /// <param name="record">Row from the query result</param>
        /// <returns>Created and populated entity.</returns>
        public override MapperProviderTests Map(IDataRecord record)
        {
            throw new NotImplementedException();
        }
    }

}
