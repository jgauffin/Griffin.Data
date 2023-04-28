using DemoApp.Core.TodoTasks;
using DemoApp.Data.TodoTasks;
using FluentAssertions;
using Griffin.Data.Mapper;

namespace DemoApp.Data.Tests.TodoTasks;

public class GithubIssueRepositoryTests : IntegrationTest
{
    private readonly GithubIssueRepository _repository;

    public GithubIssueRepositoryTests()
    {
        _repository = new GithubIssueRepository(Session);
    }

    [Fact]
    public async Task Should_be_able_to_delete_entity()
    {
        var entity = CreateValidEntity();
        await Session.Insert(entity);

        await _repository.Delete(entity);

        var actual = await Session.FirstOrDefault<GithubIssue>(new { entity.TaskId });
        actual.Should().BeNull();
    }

    [Fact]
    public async Task Should_be_able_to_insert_entity()
    {
        var entity = CreateValidEntity();

        await _repository.Create(entity);
    }

    [Fact]
    public async Task Should_be_able_to_update_entity()
    {
        var entity = CreateValidEntity();
        await Session.Insert(entity);

        await _repository.Update(entity);

        var actual = await Session.FirstOrDefault<GithubIssue>(new { entity.TaskId });
        actual.Should().NotBeNull();
        actual.Name.Should().Be(entity.Name);
        actual.IssueUrl.Should().Be(entity.IssueUrl);
    }

    private GithubIssue CreateValidEntity()
    {
        var entity = new GithubIssue(733131975, "1956", 1269523530);
        return entity;
    }
}
