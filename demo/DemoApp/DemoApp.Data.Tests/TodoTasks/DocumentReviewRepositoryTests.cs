using DemoApp.Core.TodoTasks;
using DemoApp.Data.TodoTasks;
using FluentAssertions;
using Griffin.Data.Mapper;

namespace DemoApp.Data.Tests.TodoTasks;

public class DocumentReviewRepositoryTests : IntegrationTest
{
    private readonly DocumentReviewRepository _repository;

    public DocumentReviewRepositoryTests()
    {
        _repository = new DocumentReviewRepository(Session);
    }

    [Fact]
    public async Task Should_be_able_to_delete_entity()
    {
        var entity = CreateValidEntity();
        await Session.Insert(entity);

        await _repository.Delete(entity);

        var actual = await Session.FirstOrDefault<DocumentReview>(new { entity.TaskId });
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

        entity.Comment = "8112";

        await _repository.Update(entity);

        var actual = await Session.FirstOrDefault<DocumentReview>(new { entity.TaskId });
        actual.Should().NotBeNull();
        actual.DocumentUrl.Should().Be(entity.DocumentUrl);
        actual.Comment.Should().Be(entity.Comment);
    }

    private DocumentReview CreateValidEntity()
    {
        var entity = new DocumentReview(674958112, "7523");
        return entity;
    }
}
