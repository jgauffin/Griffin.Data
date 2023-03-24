using FluentAssertions;
using Griffin.Data.Tests.Entitites;

namespace Griffin.Data.Tests;

public class CrudOperationsTests : IntegrationTests
{
    [Fact]
    public async Task Test()
    {
        var t = new MainTable2
        {
            Name = "aa",
            Age = 3,
            Rocks = true,
            Money = 5
        };
        await Transaction.Insert(Registry, t);
        await Transaction.Get<MainTable2>(Registry, t.Id);
        await Transaction.QuerySql<MainTable2>(Registry, "WHERE Money = @money", new { money = 5 });
    }


    [Fact]
    public async Task Should_be_able_to_insert_main_entity_without_children()
    {
        var sut = new MainTable
        {
            Name = "Main"
        };

        await Transaction.Insert(Registry, sut);

        var actual = await Transaction.GetById<MainTable>(Registry, sut.Id);
        actual.Name.Should().Be(sut.Name);
    }

    [Fact]
    public async Task Should_be_able_to_insert_main_entity_hasOne_child()
    {
        var sut = new MainTable
        {
            Name = "Main",
            ExtraAction = new ExtraAction { Extra = "hello" }
        };

        await Transaction.Insert(Registry, sut);

        var actual = await Transaction.GetById<MainTable>(Registry, sut.Id);
        actual.ExtraAction.Extra.Should().Be(sut.ExtraAction.Extra);
    }

    [Fact]
    public async Task Should_be_able_to_insert_main_entity_hasMany_children()
    {
        var sut = new MainTable
        {
            Name = "Main"
        };
        sut.AddLog("Hello");
        sut.AddLog("Testing");

        await Transaction.Insert(Registry, sut);

        var actual = await Transaction.GetById<MainTable>(Registry, sut.Id);
        actual.Logs[1].Message.Should().Be("Testing");
    }

    [Fact]
    public async Task Should_be_able_to_update_main_entity()
    {
        var sut = new MainTable
        {
            Name = "Main"
        };
        await Transaction.Insert(Registry, sut);

        var copy = await Transaction.GetById<MainTable>(Registry, sut.Id);
        copy.Age = 18;
        await Transaction.Update(Registry, copy);

        var actual = await Transaction.GetById<MainTable>(Registry, sut.Id);
        actual.Name.Should().Be(sut.Name);
    }

    [Fact]
    public async Task Should_be_able_to_delete_main_with_children()
    {
        var sut = new MainTable
        {
            Name = "Main",
            ExtraAction = new ExtraAction { Extra = "Hej" }
        };
        sut.AddLog("Hello");
        sut.AddLog("Testing");
        await Transaction.Insert(Registry, sut);

        await Transaction.Delete(Registry, sut);

        var actual = await Transaction.Query<Log>(Registry, new { MainId = sut.Id });
        actual.Should().BeEmpty();
        var actual1 = await Transaction.Query<Log>(Registry, new { MainId = sut.Id });
        actual1.Should().BeEmpty();
    }
}