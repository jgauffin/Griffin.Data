using Griffin.Data.Tests.Entities;

namespace Griffin.Data.IntegrationTests.Mapper;

public class InsertTests : IntegrationTests
{
    [Fact]
    public async Task Should_be_able_to_insert_hasMany_children()
    {
        var sut = new MainTable { Name = "Main" };
        sut.AddLog("Hello");
        sut.AddLog("Testing");

        await Session.Insert(sut);

        var actual = await Session.GetById<MainTable>(sut.Id);
        actual.Logs[1].Message.Should().Be("Testing");
    }

    [Fact]
    public async Task Should_be_able_to_insert_two_entities()
    {
        var sut = new MainTable { Name = "Main" };
        var sut2 = new MainTable { Name = "Main" };
        sut.AddLog("Hello");
        sut2.AddLog("Testing");

        await Session.Insert(sut);
        await Session.Insert(sut2);

        var actual1 = await Session.GetById<MainTable>(sut.Id);
        var actual2 = await Session.GetById<MainTable>(sut2.Id);
        actual1.Logs[0].Message.Should().Be("Hello");
        actual2.Logs[0].Message.Should().Be("Testing");
    }

    [Fact]
    public async Task Should_be_able_to_insert_main_entity_hasOne_child()
    {
        var sut = new MainTable
        {
            Name = "Main", 
            Children = new[]
            {
                new ChildTable { ActionType = ActionType.Disabled },
                new ChildTable { ActionType = ActionType.Extra, Action = new ExtraAction(){Extra = "Moo"}}
            }
        };

        await Session.Insert(sut);

        var actual = await Session.GetById<MainTable>(sut.Id);
        actual.Children.Count.Should().Be(2);
    }

    [Fact]
    public async Task Should_be_able_to_insert_main_entity_without_children()
    {
        var sut = new MainTable { Name = "Main" };

        await Session.Insert(sut);

        var actual = await Session.GetById<MainTable>(sut.Id);
        actual.Name.Should().Be(sut.Name);
    }

    [Fact]
    public async Task Should_be_able_to_insert_all()
    {
        var sut = new MainTable
        {
            Name = "Main",
            Children = new[]
            {
                new ChildTable { ActionType = ActionType.Disabled },
                new ChildTable { ActionType = ActionType.Extra, Action = new ExtraAction(){Extra = "Moo"}}
            }
        };
        sut.AddLog("SomeLog");
        sut.AddLog("SomeLog2");

        await Session.Insert(sut);

        var actual = await Session.GetById<MainTable>(sut.Id);
        actual.Children.Count.Should().Be(2);
        actual.Logs.Count.Should().Be(2);
    }
}
