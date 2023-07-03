using System.Reflection;
using FluentAssertions;
using Griffin.Data.ChangeTracking;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Tests.ChangeTracker.Implementations.Services.Subjects;

namespace Griffin.Data.Tests.ChangeTracker.Implementations.Services;

public class SingleEntityComparerTests
{
    [Fact]
    public void Should_detect_new_child_that_got_the_id_assigned()
    {
        var reg = new MappingRegistry();
        reg.Scan(Assembly.GetExecutingAssembly());
        var snapshot = new Root(1) { Child = new ChildWithChildren(2) { Children = new List<Child> { new() { Id = 3 } } } };
        var modified = new Root(1)
        {
            Child = new ChildWithChildren(2) { Children = new List<Child> { new() { Id = 3 }, new() { Id = 4 } } }
        };

        var sut = new SingleEntityComparer(reg);
        var result = sut.Compare(snapshot, modified);

        result.First(x => x.TrackedItem.Key == "Child4").State.Should().Be(ChangeState.Added);
    }

    [Fact]
    public void Should_detect_new_child_that_got_no_new_id_yet()
    {
        var reg = new MappingRegistry();
        reg.Scan(Assembly.GetExecutingAssembly());
        var snapshot = new Root(1) { Child = new ChildWithChildren(2) { Children = new List<Child> { new() { Id = 3 } } } };
        var modified = new Root(1)
        {
            Child = new ChildWithChildren(2) { Children = new List<Child> { new() { Id = 3 }, new() } }
        };

        var sut = new SingleEntityComparer(reg);
        var result = sut.Compare(snapshot, modified);

        result.Should().Contain(x => x.State == ChangeState.Added);
    }

}
