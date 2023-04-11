using FluentAssertions;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.Tests.ChangeTracker;

public class SingleEntityChangeServiceTests : IntegrationTests
{
    [Fact]
    public async Task Should_persist_changes_made_in_custom_entity()
    {
        var main2 = await Session.GetById<MainTable>(1);
        var main = CreateManualMain();

        var sut = new SingleEntityChangeService(Registry);
        await sut.PersistChanges(Session, main2, main);

        var result = await Session.GetById<MainTable>(main.Id);
        result.Logs[0].Message.Should().Be("Hejsan");
        result.Children[0].Action.As<ExtraAction>().Extra.Should().Be("Something new");
        result.Children[1].Action.As<SimpleAction>().Simple.Should().Be(4);
        result.Money.Should().Be(3);
    }

    [Fact]
    public async Task Should_be_able_to_generate_report()
    {
        var main2 = await Session.GetById<MainTable>(1);
        var main = CreateManualMain();
        var sut = new SingleEntityChangeService(Registry);
        
        var result = await sut.PersistChanges(Session, main2, main);

        var report = result.GenerateReport();
    }

    private static MainTable CreateManualMain()
    {
        var main = new MainTable
        {
            Id = 1,
            Name = "Mine",
            Money = 3,
            Children = new[]
            {
                new()
                {
                    MainId = 1,
                    Id = 2,
                    ActionType = ActionType.Extra,
                    Action = new ExtraAction
                    {
                        Extra = "Something new",
                        Id = 1,
                        ChildId = 2
                    }
                },
                new ChildTable { ActionType = ActionType.Simple, Action = new SimpleAction { Simple = 4 } }
            }
        };
        main.AddLog("Hejsan");
        return main;
    }
}
