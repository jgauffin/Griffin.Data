using DemoApp.Core.Accounts;
using Griffin.Data;
using Griffin.Data.Configuration;

namespace DemoApp.Data.Accounts.Mappings;

public class AccountMapping : IEntityConfigurator<Account>
{
    public void Configure(IClassMappingConfigurator<Account> config)
    {
        config.TableName("Accounts");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.UserName);
        config.Property(x => x.Password);
        config.Property(x => x.Salt);
    }
}
