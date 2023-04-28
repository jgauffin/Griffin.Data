using DemoApp.Core.TodoTasks;
using DemoApp.Data.TodoTasks;
using FluentAssertions;
using Griffin.Data.Mapper;

namespace DemoApp.Data.Tests.TodoTasks;

public class TodoTaskRepositoryTests : IntegrationTest
{
    private readonly TodoTaskRepository _repository;

    public TodoTaskRepositoryTests()
    {
        _repository = new TodoTaskRepository(Session);
    }

    [Fact]
    public async Task Should_be_able_to_delete_entity()
    {
        var entity = CreateValidEntity();
        await Session.Insert(entity);

        await _repository.Delete(entity);

        var actual = await Session.FirstOrDefault<TodoTask>(new { entity.Id });
        actual.Should().BeNull();
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

        entity.UpdatedById = 651771417;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _repository.Update(entity);

        var actual = await Session.FirstOrDefault<TodoTask>(new { entity.Id });
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

    private TodoTask CreateValidEntity()
    {
        var entity = new TodoTask(627702192, "6407", 1213213029, TodoTaskState.NotSpecified, 115986461, 730976269,
            DateTime.UtcNow);
        return entity;
    }
}
