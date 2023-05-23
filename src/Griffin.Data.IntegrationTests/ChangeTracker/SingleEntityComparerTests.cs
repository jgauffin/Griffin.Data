using Griffin.Data.ChangeTracking;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.IntegrationTests.ChangeTracker;

public class SingleEntityComparerTests : IntegrationTests
{
    [Fact]
    public async Task Should_detect_changes()
    {
        var main2 = await Session.GetById<MainTable>(1);
        var main = CreateManualMain();

        var sut = new SingleEntityComparer(Registry);
        var result = sut.Compare(main2, main);

        var actualMain = result.First(x => x.Depth == 1);
        actualMain.State.Should().Be(ChangeState.Modified);
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
