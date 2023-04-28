using Griffin.Data.Domain;

namespace DemoApp.Domain.Accounts
{
    public interface IAccountRepository : ICrudOperations<Account>
    {
        Task<Account> GetById(int id);
    }
}
