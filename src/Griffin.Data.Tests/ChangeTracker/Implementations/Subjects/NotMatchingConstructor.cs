using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Subjects
{
    internal class NotMatchingConstructor
    {
        public NotMatchingConstructor(int id)
        {

        }

        public int Age { get; set; }

    }
}
