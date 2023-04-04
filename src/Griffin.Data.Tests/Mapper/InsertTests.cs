using Griffin.Data.Mapper;
using FluentAssertions;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.Tests.Mapper
{
    public class InsertTests : IntegrationTests
    {

        [Fact]
        public async Task Should_be_able_to_insert_main_entity_without_children()
        {
            var sut = new MainTable
            {
                Name = "Main"
            };

            await Session.Insert(sut);

            var actual = await Session.GetById<MainTable>(sut.Id);
            actual.Name.Should().Be(sut.Name);
        }

        [Fact]
        public async Task Should_be_able_to_insert_main_entity_hasOne_child()
        {
            var sut = new MainTable
            {
                Name = "Main",
                Children = new[] { new ChildTable { ActionType = ActionType.Disabled } }
            };

            await Session.Insert(sut);

            var actual = await Session.GetById<MainTable>(sut.Id);
            actual.Children.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Should_be_able_to_insert_hasMany_children()
        {
            var sut = new MainTable
            {
                Name = "Main"
            };
            sut.AddLog("Hello");
            sut.AddLog("Testing");

            await Session.Insert(sut);

            var actual = await Session.GetById<MainTable>(sut.Id);
            actual.Logs[1].Message.Should().Be("Testing");
        }

    }
}
