using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.Tests.Mapper
{
    public class UpdateTests : IntegrationTests
    {


        [Fact]
        public async Task Should_be_able_to_update_main_entity()
        {
            var sut = new MainTable
            {
                Name = "Main"
            };
            await Session.Insert(sut);

            var copy = await Session.GetById<MainTable>(sut.Id);
            copy.Age = 18;
            await Session.Update(copy);

            var actual = await Session.GetById<MainTable>(sut.Id);
            actual.Name.Should().Be(sut.Name);
        }

    }
}
