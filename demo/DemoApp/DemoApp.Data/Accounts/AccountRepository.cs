using DemoApp.Core.Accounts;
using Griffin.Data;
using Griffin.Data.Domain;
using Griffin.Data.Mapper;

namespace DemoApp.Data.Accounts;

public class AccountRepository : CrudOperations<Account>, IAccountRepository
{
    public AccountRepository(Session session) : base(session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }
    }

    public async Task<Account> GetById(int id)
    {
        return await Session.First<Account>(new { id });
    }
}
