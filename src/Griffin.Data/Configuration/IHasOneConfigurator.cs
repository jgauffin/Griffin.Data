using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Allows the main class get the configuration from the generic HasOne implementation.
/// </summary>
public interface IHasOneConfigurator
{
    /// <summary>
    ///     Name of the child entity property (in the parent class).
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
    IHasOneMapping Build(IMappingRegistry mappingRegistry);
}
