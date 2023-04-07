using System;

namespace Griffin.Data.Mappings;

/// <summary>
///     Registry which all loaded mappings are stored in.
/// </summary>
public interface IMappingRegistry
{
    /// <summary>
    ///     Get a mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to get mapping for.</typeparam>
    /// <returns>Mapping.</returns>
    /// <exception cref="InvalidOperationException">Mapping was not found.</exception>
    ClassMapping Get<TEntity>();

    /// <summary>
    ///     Get a mapping
    /// </summary>
    /// <param name="type">Type of entity to get mapping for.</param>
    /// <returns>Mapping.</returns>
    /// <exception cref="InvalidOperationException">Mapping was not found.</exception>
    ClassMapping Get(Type type);
}
