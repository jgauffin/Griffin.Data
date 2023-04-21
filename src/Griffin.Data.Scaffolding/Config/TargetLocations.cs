namespace Griffin.Data.Scaffolding.Config;

/// <summary>
///     Namespaces that different types of generated classes should be placed in.
/// </summary>
internal class TargetLocations
{
    /// <summary>
    ///     Where entities should be located.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Entities.[PluralEntityName]</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? Entities { get; set; }

    /// <summary>
    ///     Where entity mappings should be placed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Data.[PluralEntityName]</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? Mappings { get; set; }

    /// <summary>
    ///     Where queries should be placed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Data.Queries</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? Queries { get; set; }

    /// <summary>
    ///     Where repositories should be placed.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Data.[PluralEntityName]</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? RepositoryClasses { get; set; }

    /// <summary>
    ///     Where repository interfaces should be located.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Entities.[PluralEntityName]</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? RepositoryInterfaces { get; set; }

    /// <summary>
    ///     Where tests should be located.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>rootNamespace.Data.Tests</c>
    ///     </para>
    /// </remarks>
    public TargetLocation? Tests { get; set; }
}
