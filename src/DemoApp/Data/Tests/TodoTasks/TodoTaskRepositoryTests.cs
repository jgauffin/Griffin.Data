using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Domain.TodoTasks;
using DemoApp.Data.TodoTasks;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data\Tests.TodoTasks
{
    public class TodoTaskRepositoryTests : IntegrationTest
    {
        private readonly TodoTaskRepository _repository;

        public TodoTaskRepositoryTests()
        {
            _repository = new TodoTaskRepository(Session);
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
            
            entity.UpdatedById = 416886079;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.Update(entity);

            var actual = await Session.FirstOrDefault<TodoTask>(new {entity.Id});
            actual.Should().NotBeNull();
            actual.TodolistId.Should().Be(entity.TodolistId);
            actual.Name.Should().Be(entity.Name);
            actual.TaskType.Should().Be(entity.TaskType);
            actual.State.Should().Be(entity.State);
            actual.Priority.Should().Be(entity.Priority);
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

            var actual = await Session.FirstOrDefault<TodoTask>(new {entity.Id});
            actual.Should().BeNull();
        }

        private TodoTask CreateValidEntity()
        {
            var entity = new TodoTask(748926544, "3901", 695349778, TodoTaskState.NotSpecified, 2126834587, 1477445036, DateTime.UtcNow);
            return entity;
        }

    }
}
