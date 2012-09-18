using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace Griffin.Data.Tests.BasicLayer
{
    public class CommandExtensionTests
    {
        public void Test()
        {
            var cmd = Substitute.For<IDbCommand>();
        }
    }
}
