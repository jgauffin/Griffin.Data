using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Configuration.Subjects
{
    internal class UserConfigurator : IEntityConfigurator<User>
    {
        public void Configure(IClassMappingConfigurator<User> config)
        {
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.FirstName);
            
            config.HasMany(x=>x.Addresses)
                .ForeignKey(x=>x.UserId)
                .References(x=>x.Id);
            
            config.HasOne(x=>x.Data)
                .Denominator(x=>x.State, CreateChildEntity)
                .ForeignKey(x=>x.UserId)
                .References(x=>x.Id);
            
        }

        private Data? CreateChildEntity(AccountState arg)
        {
            switch (arg)
            {
                case AccountState.Active:
                    return new UserData();
                case AccountState.Admin:
                    return new AdminData();
                default:
                    return null;
            }
        }
    }
}
