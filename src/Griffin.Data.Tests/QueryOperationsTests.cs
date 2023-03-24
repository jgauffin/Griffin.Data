using FluentAssertions;
using Griffin.Data.Tests.Entitites;

namespace Griffin.Data.Tests;

public class QueryOperationsTests : IntegrationTests
{
    [Fact]
    public async Task Should_be_able_to_load_enum()
    {
        var item = await Transaction.GetById<MainTable>(Registry, 1);

        item.ActionType.Should().Be(MainActionType.Disabled);
    }
}