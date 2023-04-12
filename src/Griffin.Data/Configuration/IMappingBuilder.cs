using Griffin.Data.Mappings;

namespace Griffin.Data.Configuration;

/// <summary>
///     Builds a mapping from a configuration.
/// </summary>
/// <remarks>
///     <para>
///         Mappings should be built in two steps. First create all mappings and next build all relations (as relations
///         can't be built unless all mappings exists).
///     </para>
/// </remarks>
public interface IMappingBuilder
{
    /// <summary>
    ///     Build mapping
    /// </summary>
    /// <param name="pluralizeTableNames">Table names should be pluralized versions of the class names.</param>
    /// <returns>Generated mapping.</returns>
    ClassMapping BuildMapping(bool pluralizeTableNames);

    /// <summary>
    ///     Build relations between entities.
    /// </summary>
    /// <param name="registry">Mapping registry where all mappings have been built.</param>
    void BuildRelations(IMappingRegistry registry);
}
