using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.Tests.Subjects
{
    internal class SharedMain
    {
        public int SomeField { get; set; }
        public SharedChild Left { get; set; }
        public SharedChild Right { get; set; }
        public int Id { get; set; }
    }

    public class SharedChild
    {
        public int Id { get; set; }
        public int MainId { get; set; }
        public string Value { get; set; }
    }
}
