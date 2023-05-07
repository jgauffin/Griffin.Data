using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Core.TodoTasks;
using DemoApp.Data.TodoTasks;

namespace DemoApp.Data.Tests.TodoTasks
{
    public class GithubIssueRepositoryTests : IntegrationTest
    {
        private readonly GithubIssueRepository _repository;

        public GithubIssueRepositoryTests()
        {
            _repository = new GithubIssueRepository(Session);
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

            var actual = await Session.FirstOrDefault<GithubIssue>(new {entity.TaskId});
            actual.Should().NotBeNull();
            actual.Name.Should().Be(entity.Name);
            actual.IssueUrl.Should().Be(entity.IssueUrl);
        }

        [Fact]
        public async Task Should_be_able_to_delete_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);
            
            await _repository.Delete(entity);

            var actual = await Session.FirstOrDefault<GithubIssue>(new {entity.TaskId});
            actual.Should().BeNull();
        }

        private GithubIssue CreateValidEntity()
        {
            var entity = new GithubIssue(1650235913, "2658", 815666346);
            return entity;
        }

    }
}
