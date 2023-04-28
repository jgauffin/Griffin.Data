namespace DemoApp.Core.Accounts;

public interface IAccountRepository
{
    Task Create(Account entity);

    Task Delete(Account entity);
    Task<Account> GetById(int id);

    Task Update(Account entity);
}
