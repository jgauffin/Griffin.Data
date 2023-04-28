using Griffin.Data;
using Griffin.Data.Configuration;
using D:\src\jgauffin\Griffin.Data\src\DemoApp\Domain.Todolists;

namespace D:\src\jgauffin\Griffin.Data\src\DemoApp\Data.Todolists.Mappings
{
    public class PermissionMapping : IEntityConfigurator<Permission>
    {
        public void Configure(IClassMappingConfigurator<Permission> config)
        {
            config.TableName("TodolistPermissions");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.TodolistId);
            config.Property(x => x.AccountId);
            config.Property(x => x.CanRead);
            config.Property(x => x.CanWrite);
            config.Property(x => x.IsAdmin);
        }
    }
}
