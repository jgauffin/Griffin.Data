using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects
{
    internal class UserConfigurator : IEntityConfigurator<User>
    {
        public void Configure(IClassMappingConfigurator<User> config)
        {
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.FirstName);

            config.HasMany(x => x.Addresses)
                .ForeignKey(x => x.UserId)
                .References(x => x.Id);

            config.HasOne(x => x.Data)
                .Discriminator(x => x.State, CreateChildEntity)
                .ForeignKey(x => x.UserId)
                .References(x => x.Id);

        }

        private Type? CreateChildEntity(AccountState arg)
        {
            return arg switch
            {
                AccountState.Active => typeof(UserData),
                AccountState.Admin => typeof(AdminData),
                _ => null
            };
        }
    }
}
