using Griffin.Data.Domain;

namespace DemoApp.Domain.Todolists
{
    public interface IPermissionRepository : ICrudOperations<Permission>
    {
        Task<Permission> GetById(int id);
    }
}
