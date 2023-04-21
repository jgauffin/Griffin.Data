using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace Griffin.Data.Domain.Implementation;

/// <summary>
///     Performs CRUD for entities.
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
/// <remarks>
///     <para>
///         Typically used as a base class for repositories.
///     </para>
/// </remarks>
public class CrudOperations<TEntity> : ICrudOperations<TEntity>
{
    private readonly Session _session;

    /// <summary>
    /// </summary>
    /// <param name="session">Session sued to </param>
    /// <exception cref="ArgumentNullException"></exception>
    public CrudOperations(Session session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <inheritdoc />
    public async Task Create([DisallowNull] TEntity entity)
    {
        await _session.Insert(entity);
    }

    /// <inheritdoc />
    public async Task Delete([DisallowNull] TEntity entity)
    {
        await _session.Delete(entity);
    }

    /// <inheritdoc />
    public async Task Update([DisallowNull] TEntity entity)
    {
        await _session.Update(entity);
    }
}
