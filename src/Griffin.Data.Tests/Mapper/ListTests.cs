using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mapper;

public class ListTests : IntegrationTests
{
    [Fact]
    public async Task Should_work_with_just_sql_and_options()
    {

        var items = await Session.List<SharedMain>("SELECT TOP(1) * FROM SharedMain WHERE Id = @id ORDER BY Id", new { id = 1 });

        items.Should().HaveCount(1);
        items[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task Should_work_with_just_sql()
    {

        var items = await Session.List<SharedMain>("SELECT TOP(1) * FROM SharedMain WHERE Id = 1 ORDER BY Id");

        items.Should().HaveCount(1);
        items[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task Should_work_with_generic_query_using_sql()
    {
        var items = await Session.Query<SharedMain>()
            .Where("SELECT TOP(1) * FROM SharedMain WHERE Id = @id ORDER BY Id", new { id = 1 })
            .Paging(1, 50)
            .List();

        items.Should().HaveCount(1);
        items[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task Should_be_able_to_limit_result()
    {
        var options = Session.Query<SharedChild>()
            .Where(new { MainId = 1 })
            .Paging(1, 1);

        var children = await Session.List(options);

        children.Should().HaveCount(1);
        children[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task Should_be_able_to_load_multiple_items()
    {
        var options = Session.Query<SharedChild>().Where(new { MainId = 1 });

        var children = await Session.List(options);

        children.Should().HaveCount(2);
        children[0].Value.Should().Be("Stop");
    }

    [Fact]
    public async Task Should_be_able_to_page_result()
    {
        var options = Session.Query<SharedChild>()
            .Where(new { MainId = 1 })
            .Paging(2, 1);

        var children = await Session.List(options);

        children.Should().HaveCount(1);
        children[0].Id.Should().Be(2);
    }
}
