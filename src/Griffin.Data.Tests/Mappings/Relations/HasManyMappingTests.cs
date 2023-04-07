using FluentAssertions;
using Griffin.Data.Mappings.Relations;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings.Relations;

public class HasManyMappingTests
{
    private readonly IHasManyMapping _hasManyMapping;

    public HasManyMappingTests()
    {
        var reg = RegistryBuilder.Build();
        _hasManyMapping = reg.Get<User>().Collections[0];
    }

    [Fact]
    public void Should_be_able_to_traverse_collection()
    {
        var collection = _hasManyMapping.CreateCollection();
        collection.Add(new Address());
        collection.Add(new Address());
        var actual = 0;

        _hasManyMapping.Visit(collection, x =>
        {
            actual++;
            return Task.CompletedTask;
        });

        actual.Should().Be(2);
    }

    [Fact]
    public void Should_create_collection_of_correct_type()
    {
        var actual = _hasManyMapping.CreateCollection();

        actual.GetType().GetGenericArguments()[0].Should().Be(typeof(Address));
    }

    [Fact]
    public void Should_point_at_element_and_not_list_type()
    {
        _hasManyMapping.ChildEntityType.Should().Be<Address>();
    }
}
