namespace Griffin.Data.Scaffolding.Config;

/// <summary>
///     Scaffolder configuration.
/// </summary>
/// <remarks>
///     <para>
///         Used to tell the scaffolder what to generate and when to do it. The configuration can either be
///         manually specified or by adding a <c>griffin.data.config.json</c> to your class library.
///     </para>
/// </remarks>
internal class ScaffoldingConfiguration
{
    public const string Filename = "datamapper.config.json";

    /// <summary>
    ///     Connection string to use when reading information from the database.
    /// </summary>
    /// <remarks>
    ///     <para>Should be a standard ADO.NET connection string.</para>
    /// </remarks>
    public string ConnectionString { get; set; } = "";

    /// <summary>
    ///     Entities to generate.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is an alternative to <see cref="Tables" /> when you want to tell which entities to generate and where the
    ///         they get located.
    ///     </para>
    /// </remarks>
    public ICollection<ScaffoldingEntityConfiguration>? Entities { get; } = new List<ScaffoldingEntityConfiguration>();

    /// <summary>
    ///     Name of the ADO.NET Provider (default is SqlServer).
    /// </summary>
    public string? SqlProviderName { get; set; }

    /// <summary>
    ///     Tables to generate entities from.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This property is used when you want to generate everything based on the standard conventions. Use a single
    ///         entry with <c>*</c> to generate for all tables.
    ///     </para>
    ///     <para>Use <see cref="TypesToGenerate" /> to specify which types of classes to scaffold.</para>
    /// </remarks>
    public ICollection<string>? Tables { get; } = new List<string>();

    /// <summary>
    ///     Where different types of classes are generated.
    /// </summary>
    public TargetLocations? TargetLocations { get; set; }

    /// <summary>
    ///     Type of classes to scaffold.
    /// </summary>
    public ICollection<TypeToGenerate>? TypesToGenerate { get; } = new List<TypeToGenerate>();
}
