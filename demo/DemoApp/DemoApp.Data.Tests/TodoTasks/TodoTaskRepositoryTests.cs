using DemoApp.Core.Accounts;
using DemoApp.Core.Todolists;
using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Core.TodoTasks;
using DemoApp.Data.TodoTasks;

namespace DemoApp.Data.Tests.TodoTasks
{
    public class TodoTaskRepositoryTests : IntegrationTest
    {
        private readonly TodoTaskRepository _repository;
        private Account _account = new Account("jgauffin", "123456", "sprinkled");
        private Todolist _list;

        public TodoTaskRepositoryTests()
        {
            Session.Insert(_account).Wait();
            _list = new Todolist(_account.Id, DateTime.UtcNow);
            Session.Insert(_list).Wait();
            _repository = new TodoTaskRepository(Session);
        }

        [Fact]
        public async Task Should_be_able_to_insert_entity()
        {
            var entity = CreateValidEntity();

            await _repository.Create(entity);

            var id = await Session.FirstOrDefault<Todolist>(new { Id = 23 });
            
            entity.Id.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task Should_be_able_to_update_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);

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

            var actual = await Session.FirstOrDefault<TodoTask>(new {entity.Id});
            actual.Should().BeNull();
        }

        private TodoTask CreateValidEntity()
        {
            
            var entity = new TodoTask(_list.Id, "3673", 1, TodoTaskState.NotSpecified, 1220926417, _account.Id, DateTime.UtcNow);
            return entity;
        }

    }
}
