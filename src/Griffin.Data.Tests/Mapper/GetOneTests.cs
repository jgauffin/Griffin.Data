using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mapper;

public class GetOneTests : IntegrationTests
{
    [Fact]
    public async Task Should_be_able_to_load_long()
    {
        var item = await Session.GetById<MainTable>(1);

        item.Money.Should().Be(39093289238);
    }

    [Fact]
    public async Task Should_get_child_entities_per_default()
    {
        var actual = await Session.GetById<MainTable>(1);

        actual.Children.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_get_children_recursively()
    {
        var actual = await Session.GetById<MainTable>(1);

        actual.Children[0].Action.Should().BeOfType<SimpleAction>();
    }

    [Fact]
    public async Task Should_get_children_recursively_and_allow_children_to_have_no_configured_child()
    {
        var actual = await Session.GetById<MainTable>(1);

        actual.Children[1].Action.Should().BeNull();
    }

    [Fact]
    public async Task Should_get_specific_children_when_using_subset_column_for_hasMany()
    {
        var actual = await Session.GetById<SharedMainCollection>(1);

        actual.Left[0].Value.Should().Be("Stop");
        actual.Right[0].Value.Should().Be("Skip");
        actual.Left.Should().HaveCount(1);
        actual.Right.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_get_specific_children_when_using_subset_column_for_hasOne()
    {
        var actual = await Session.GetById<SharedMain>(1);

        actual.Left.Value.Should().Be("Stop");
        actual.Right.Value.Should().Be("Skip");
    }

    [Fact]
    public async Task Should_not_get_child_entities_when_turned_off()
    {
        var actual = await Session.First<MainTable>(new QueryOptions("", new { Id = 1 }) { LoadChildren = false });

        actual.Children.Should().BeEmpty();
    }
}
