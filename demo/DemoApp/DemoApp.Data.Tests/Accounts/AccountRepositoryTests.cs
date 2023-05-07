using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;
using DemoApp.Core.Accounts;
using DemoApp.Data.Accounts;

namespace DemoApp.Data.Tests.Accounts
{
    public class AccountRepositoryTests : IntegrationTest
    {
        private readonly AccountRepository _repository;

        public AccountRepositoryTests()
        {
            _repository = new AccountRepository(Session);
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

            var actual = await Session.FirstOrDefault<Account>(new {entity.Id});
            actual.Should().NotBeNull();
            actual.UserName.Should().Be(entity.UserName);
            actual.Password.Should().Be(entity.Password);
            actual.Salt.Should().Be(entity.Salt);
        }

        [Fact]
        public async Task Should_be_able_to_delete_entity()
        {
            var entity = CreateValidEntity();
            await Session.Insert(entity);
            
            await _repository.Delete(entity);

            var actual = await Session.FirstOrDefault<Account>(new {entity.Id});
            actual.Should().BeNull();
        }

        private Account CreateValidEntity()
        {
            var entity = new Account("3879", "7870", "8815");
            return entity;
        }

    }
}
