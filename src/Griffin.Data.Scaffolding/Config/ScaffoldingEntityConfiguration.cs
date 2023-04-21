namespace Griffin.Data.Scaffolding.Config;

/// <summary>
///     Configuration
/// </summary>
public class ScaffoldingEntityConfiguration
{
    public ScaffoldingEntityConfiguration(string tableName)
    {
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
    }

    /// <summary>
    ///     Directory to create the entity in (can be used to override default location).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is the same as the namespace.
    ///     </para>
    /// </remarks>
    public string? Directory { get; set; }

    /// <summary>
    ///     Name of entity.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is the table name in singular form and pascal case.
    ///     </para>
    /// </remarks>
    public string? EntityName { get; set; }

    /// <summary>
    ///     Namespace to place the entity in (can be used to override default location).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is <c>[rootNamespace].Entities</c>
    ///     </para>
    /// </remarks>
    public string? Namespace { get; set; }

    /// <summary>
    ///     Table to read from.
    /// </summary>
    public string TableName { get; }
}
