using DemoApp.Core.Todolists;
using Griffin.Data;
using Griffin.Data.Domain;
using Griffin.Data.Mapper;

namespace DemoApp.Data.Todolists;

public class PermissionRepository : CrudOperations<Permission>, IPermissionRepository
{
    public PermissionRepository(Session session) : base(session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }
    }

    public async Task<Permission> GetById(int id)
    {
        return await Session.First<Permission>(new { id });
    }
}
