using DemoApp.Core;

namespace DemoApp.Core.Todolists
{
    public interface IPermissionRepository
    {
        Task<Permission> GetById(int id);

        Task Create(Permission entity);

        Task Update(Permission entity);

        Task Delete(Permission entity);

    }
}
