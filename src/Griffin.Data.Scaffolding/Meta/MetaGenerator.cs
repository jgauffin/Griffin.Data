using Griffin.Data.Dialects;

namespace Griffin.Data.Scaffolding.Meta;

internal class MetaGenerator
{
    public async Task<IReadOnlyList<Table>> ReadSchema(ISqlDialect dialect, string connectionString)
    {
        if (dialect == null)
        {
            throw new ArgumentNullException(nameof(dialect));
        }

        var reader = dialect.CreateSchemaReader();
        var tables = new List<Table>();
        var context = new SchemaReaderContext(tables);

        using var connection = dialect.CreateConnection();
        connection.ConnectionString = connectionString;
        connection.Open();

        await reader.ReadSchema(connection, context);

        return tables;
    }
}
