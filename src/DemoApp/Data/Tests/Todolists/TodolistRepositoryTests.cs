using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Domain.Todolists;
using DemoApp.Data.Todolists;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data\Tests.Todolists
{
    public class TodolistRepositoryTests : IntegrationTest
    {
        private readonly TodolistRepository _repository;

        public TodolistRepositoryTests()
        {
            _repository = new TodolistRepository(Session);
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
            
            entity.UpdatedById = 413453813;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.Update(entity);

            var actual = await Session.FirstOrDefault<Todolist>(new {entity.Id});
            actual.Should().NotBeNull();
            actual.Name.Should().Be(entity.Name);
            actual.CreatedById.Should().Be(entity.CreatedById);
            actual.CreatedAtUtc.Should().Be(entity.CreatedAtUtc);
            actual.UpdatedById.Should().Be(entity.UpdatedById);
            actual.UpdatedAtUtc.Should().Be(entity.UpdatedAtUtc);
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
            var entity = new Todolist("780", 1100640808, DateTime.UtcNow);
            return entity;
        }

    }
}
