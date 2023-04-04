using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;

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
    public async Task Should_not_get_child_entities_when_turned_off()
    {
        var actual = await Session.First<MainTable>(new QueryOptions("", new { Id = 1 }) { LoadChildren = false });

        actual.Children.Should().BeEmpty();
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

}