using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Core.Todolists;
using DemoApp.Data.Todolists;
using DemoApp.Core.Accounts;

namespace DemoApp.Data.Tests.Todolists
{
    public class TodolistRepositoryTests : IntegrationTest
    {
        private readonly TodolistRepository _repository;
        private Account _account = new Account("jgauffin", "123456", "sprinkled");

        public TodolistRepositoryTests()
        {
            _repository = new TodolistRepository(Session);
            Session.Insert(_account).Wait();
        }

        [Fact]
        public async Task Should_be_able_to_insert_entity()
        {
            var entity = CreateValidEntity();

            await _repository.Create(entity);
            
            entity.Id.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task Should_be_able_to_update_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);
            
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.Update(entity);

            var actual = await Session.FirstOrDefault<Todolist>(new {entity.Id});
            actual.Should().NotBeNull();
            actual.Name.Should().Be(entity.Name);
            actual.CreatedById.Should().Be(entity.CreatedById);
            actual.CreatedAtUtc.Should().BeCloseTo(entity.CreatedAtUtc, TimeSpan.FromMilliseconds(100));
            actual.UpdatedById.Should().Be(entity.UpdatedById);
            actual.UpdatedAtUtc.Should().BeCloseTo(entity.UpdatedAtUtc.Value, TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public async Task Should_be_able_to_delete_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);
            
            await _repository.Delete(entity);

            var actual = await Session.FirstOrDefault<Todolist>(new {entity.Id});
            actual.Should().BeNull();
        }

        private Todolist CreateValidEntity()
        {
            //var entity = new Todolist("5344", _account.Id, DateTime.UtcNow);
            var entity = new Todolist(_account.Id, DateTime.UtcNow);
            return entity;
        }

    }
}
