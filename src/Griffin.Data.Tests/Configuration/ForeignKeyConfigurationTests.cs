using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Mappings.Relations;
using Griffin.Data.Tests.Configuration.Subjects;

namespace Griffin.Data.Tests.Configuration
{
    public class ForeignKeyConfigurationTests
    {
        [Fact]
        public void Should_set_correct_referenced_property()
        {
            var mapping = new ForeignKeyMapping(typeof(User), "Users");

            var sut = new ForeignKeyConfiguration<User>(mapping);
            sut.References(x=>x.Id);

            mapping.ReferencedPropertyName.Should().Be("Id");
        }
    }
}
