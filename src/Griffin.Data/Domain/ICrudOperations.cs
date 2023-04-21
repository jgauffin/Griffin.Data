using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Griffin.Data.Domain;

/// <summary>
///     Interface used to define Create, Update and Delete methods.
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
public interface ICrudOperations<in TEntity>
{
    /// <summary>
    ///     Create a new entity in the database.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    /// <returns>Task</returns>
    Task Create([DisallowNull] TEntity entity);

    /// <summary>
    ///     Delete an existing entity from the database.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>Task</returns>
    Task Delete([DisallowNull] TEntity entity);

    /// <summary>
    ///     Update an existing entity in the database.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <returns>Task</returns>
    Task Update([DisallowNull] TEntity entity);
}
