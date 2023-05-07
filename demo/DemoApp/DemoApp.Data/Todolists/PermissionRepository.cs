using Griffin.Data;
using Griffin.Data.Mapper;
using Griffin.Data.Domain;
using DemoApp.Core.Todolists;

namespace DemoApp.Data.Todolists
{
    public class PermissionRepository : CrudOperations<Permission>, IPermissionRepository
    {
        public PermissionRepository(Session session) : base(session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
        }
        public async Task<Permission> GetById(int id)
        {
            return await Session.First<Permission>(new {id});
        }
    }
}
