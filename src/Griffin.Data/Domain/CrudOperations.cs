using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace Griffin.Data.Domain;

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
    /// <summary>
    /// </summary>
    /// <param name="session">Session sued to </param>
    /// <exception cref="ArgumentNullException"></exception>
    public CrudOperations(Session session)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <summary>
    ///     Session used.
    /// </summary>
    protected Session Session { get; }

    /// <inheritdoc />
    public async Task Create([DisallowNull] TEntity entity)
    {
        await Session.Insert(entity);
    }

    /// <inheritdoc />
    public async Task Delete([DisallowNull] TEntity entity)
    {
        await Session.Delete(entity);
    }

    /// <inheritdoc />
    public async Task Update([DisallowNull] TEntity entity)
    {
        await Session.Update(entity);
    }
}
