using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Allows the main class get the configuration from the generic HasMany implementation.
/// </summary>
public interface IHasManyConfigurator
{
    /// <summary>
    ///     Name of the collection property in the parent entity.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    ///     Build a complete mapping.
    /// </summary>
    /// <param name="mappingRegistry">The mapping registry.</param>
    /// <returns>Complete mapping.</returns>
    /// <remarks>
    ///     <para>
    ///         Invoke it once the mapping registry contains all mappings, or the reference generation wont work.
    ///     </para>
    /// </remarks>
    IHasManyMapping Build(IMappingRegistry mappingRegistry);
}
