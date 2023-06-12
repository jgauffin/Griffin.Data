using Griffin.Data.Logging;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;
using Log = Griffin.Data.Tests.Entities.Log;

namespace Griffin.Data.IntegrationTests.Mapper;

//TODO: Delete a row with two properties of the same type that are different with the help of a subset column
public class DeleteTests : IntegrationTests
{
    [Fact]
    public async Task Should_be_able_to_delete_main_with_children()
    {
        var sut = new MainTable
        {
            Name = "Main", Children = new[] { new ChildTable { ActionType = ActionType.Disabled } }
        };
        sut.AddLog("Hello");
        sut.AddLog("Testing");
        await Session.Insert(sut);

        await Session.Delete(sut);

        var actual = await Session.List<Log>(new { MainId = sut.Id });
        actual.Should().BeEmpty();
        var actual1 = await Session.List<Log>(new { MainId = sut.Id });
        actual1.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_delete_all_children()
    {
        var sut = new MainTable
        {
            Children = new[]
            {
                new ChildTable { ActionType = ActionType.Extra, Action = new ExtraAction { Extra = "333" } }
            }
        };
        await Session.Insert(sut);
        var g = await Session.GetById<MainTable>(sut.Id);

        await Session.Delete(sut);

        var where = QueryOptions.Where(new { sut.Children[0].Action.As<ExtraAction>().Id });
        var child = await Session.FirstOrDefault<ExtraAction>(where);
        child.Should().BeNull();
    }
}
