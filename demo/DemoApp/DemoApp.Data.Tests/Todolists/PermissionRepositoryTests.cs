using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Core.Todolists;
using DemoApp.Data.Todolists;

namespace DemoApp.Data.Tests.Todolists
{
    public class PermissionRepositoryTests : IntegrationTest
    {
        private readonly PermissionRepository _repository;

        public PermissionRepositoryTests()
        {
            _repository = new PermissionRepository(Session);
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
            

            await _repository.Update(entity);

            var actual = await Session.FirstOrDefault<Permission>(new {entity.Id});
            actual.Should().NotBeNull();
            actual.TodolistId.Should().Be(entity.TodolistId);
            actual.AccountId.Should().Be(entity.AccountId);
            actual.CanRead.Should().Be(entity.CanRead);
            actual.CanWrite.Should().Be(entity.CanWrite);
            actual.IsAdmin.Should().Be(entity.IsAdmin);
        }

        [Fact]
        public async Task Should_be_able_to_delete_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);
            
            await _repository.Delete(entity);

            var actual = await Session.FirstOrDefault<Permission>(new {entity.Id});
            actual.Should().BeNull();
        }

        private Permission CreateValidEntity()
        {
            var entity = new Permission(1435125467, 65959170, true, true, true);
            return entity;
        }

    }
}
