using FluentAssertions;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Tests.ChangeTracker.Implementations.Subjects;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services
{
    public class SingleEntityTraverserTests
    {
        private readonly MappingRegistry _registry;

        public SingleEntityTraverserTests()
        {
            _registry = new MappingRegistry();
        }

        [Fact]
        public void Should_have_null_as_parent()
        {
            var sut = new SingleEntityTraverser(_registry);

            var entities = sut.Traverse(new SomeClass(4));

            entities[0].Parent.Should().BeNull();
        }

        [Fact]
        public void Should_have_root_as_childs_parent()
        {
            var sut = new SingleEntityTraverser(_registry);
            var entity = new WithChild { Age = 4, Child = new SomeClass(5) };

            var entities = sut.Traverse(entity);

            entities[1].Parent!.Entity.Should().Be(entity);
        }
    }
}
