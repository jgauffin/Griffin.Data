using System.Reflection;
using FluentAssertions;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Tests.Helpers;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings
{
    public class ConstructorBuilderTests
    {

        [Fact]
        public void Should_use_read_parameter_and_not_build_it_as_a_constant_expression_in_the_exception_builder()
        {
            var reg = new MappingRegistry();
            reg.Scan(Assembly.GetExecutingAssembly());
            var mapping = reg.Get<ClassWithConstructor>();
            var record1 = new FakeRecord(new Dictionary<string, object> { { "Id", 1 }, { "Name", DBNull.Value } });
            var record2 = new FakeRecord(new Dictionary<string, object> { { "Id", 2 }, { "Name", DBNull.Value } });
            ConstructorBuilder builder = new ConstructorBuilder(mapping);
            var method = builder.CreateConstructor()!;
            var actual1 = () =>
            {
                method(record1);
                record1.IsDisposed = true;
            };

            var actual2 = () => method(record2);

            actual1.Should().Throw<MappingException>();
            actual2.Should().Throw<MappingException>();
        }

    }
}
