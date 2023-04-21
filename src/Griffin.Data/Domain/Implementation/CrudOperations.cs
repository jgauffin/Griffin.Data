using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace Griffin.Data.Domain.Implementation;

internal class CrudOperations<TEntity> : ICrudOperations<TEntity>
{
    private readonly Session _session;

    public CrudOperations(Session session)
    {
        _session = session;
    }

    public async Task Create([DisallowNull] TEntity entity)
    {
        await _session.Insert(entity);
    }

    public async Task Delete([DisallowNull] TEntity entity)
    {
        await _session.Delete(entity);
    }

    public async Task Update([DisallowNull] TEntity entity)
    {
        await _session.Update(entity);
    }
}
