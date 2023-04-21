namespace Griffin.Data.Scaffolding.Meta;

/// <summary>
///     Metadata for <see cref="MetaGenerator" />.
/// </summary>
internal class EngineMeta
{
    public EngineMeta(string dbEngineName, string sqlClientFactoryName, Type schemaReaderType)
    {
        DbEngineName = dbEngineName ?? throw new ArgumentNullException(nameof(dbEngineName));
        SqlClientFactoryName = sqlClientFactoryName ?? throw new ArgumentNullException(nameof(sqlClientFactoryName));
        SchemaReaderType = schemaReaderType ?? throw new ArgumentNullException(nameof(schemaReaderType));
    }

    /// <summary>
    ///     Human friendly name of the database engine (used when commanding the scaffolder)
    /// </summary>
    public string DbEngineName { get; }

    /// <summary>
    ///     Type of <see cref="ISchemaReader" />.
    /// </summary>
    public Type SchemaReaderType { get; }

    /// <summary>
    ///     ADO.NET Provider client.
    /// </summary>
    public string SqlClientFactoryName { get; }
}
