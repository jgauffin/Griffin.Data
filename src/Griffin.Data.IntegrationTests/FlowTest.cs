using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.IntegrationTests;

public class FlowTest : IntegrationTests
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
        await Session.Insert(t);
        await Session.GetById<MainTable2>(t.Id);
        await Session.List<MainTable2>("Money = @money", new { money = 5 });
    }
}
