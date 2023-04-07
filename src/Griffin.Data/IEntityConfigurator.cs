using Griffin.Data.Configuration;

namespace Griffin.Data;

/// <summary>
///     Implement this class to create a mapping.
/// </summary>
/// <typeparam name="TEntity">Type of entity to create a mapping fr.</typeparam>
public interface IEntityConfigurator<TEntity> where TEntity : notnull
{
    /// <summary>
    ///     Configure a mapping.
    /// </summary>
    /// <param name="config">Configuration class.</param>
    void Configure(IClassMappingConfigurator<TEntity> config);
}
