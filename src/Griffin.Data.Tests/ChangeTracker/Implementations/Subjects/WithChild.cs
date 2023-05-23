using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Subjects
{
    internal class WithChild
    {
        public int Age { get; set; }
        public SomeClass Child { get; set; }
    }

    class WithChildMapping : IEntityConfigurator<WithChild>
    {
        public void Configure(IClassMappingConfigurator<WithChild> config)
        {
            config.MapRemainingProperties();
        }
    }
}
