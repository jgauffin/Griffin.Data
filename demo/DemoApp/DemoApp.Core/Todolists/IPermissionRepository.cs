namespace DemoApp.Core.Todolists;

public interface IPermissionRepository
{
    Task Create(Permission entity);

    Task Delete(Permission entity);
    Task<Permission> GetById(int id);

    Task Update(Permission entity);
}
