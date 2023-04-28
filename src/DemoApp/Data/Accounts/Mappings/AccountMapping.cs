using Griffin.Data;
using Griffin.Data.Configuration;
using D:\src\jgauffin\Griffin.Data\src\DemoApp\Domain.Accounts;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data.Accounts.Mappings
{
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
}
