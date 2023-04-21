using System.Threading.Tasks;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Reads table meta data from the database,
/// </summary>
public interface ISchemaReader
{
    /// <summary>
    ///     Read schema and generate meta data.
    /// </summary>
    /// <param name="context">Context to add tables to.</param>
    /// <returns>Task.</returns>
    Task ReadSchema(SchemaReaderContext context);
}
