using DemoApp.Core;

namespace DemoApp.Core.Accounts
{
    public interface IAccountRepository
    {
        Task<Account> GetById(int id);

        Task Create(Account entity);

        Task Update(Account entity);

        Task Delete(Account entity);

    }
}
