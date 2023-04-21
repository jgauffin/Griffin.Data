using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Context for <see cref="ISchemaReader" /> implementations.
/// </summary>
public class SchemaReaderContext
{
    private static readonly Regex RxCleanUp = new(@"[^\w\d_]", RegexOptions.Compiled);
    private readonly List<Table> _tables;

    /// <summary>
    /// </summary>
    /// <param name="connectionString">Connection string supplied by the developer.</param>
    /// <param name="tables">Collection to fill with meta data.</param>
    public SchemaReaderContext(string connectionString, List<Table> tables)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _tables = tables ?? throw new ArgumentNullException(nameof(tables));
    }

    /// <summary>
    ///     Connection string supplied by the developer.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    ///     Add another table.
    /// </summary>
    /// <param name="table">Table metadata.</param>
    /// <exception cref="ArgumentNullException">Table is null.</exception>
    public void Add(Table table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        _tables.Add(table);
    }

    /// <summary>
    ///     Clean up table name.
    /// </summary>
    /// <param name="name">Name to clean up.</param>
    /// <returns>Cleaned string</returns>
    public string Cleanup(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var str = RxCleanUp.Replace(name, "_");
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }

        return str;
    }

    private string GetNullableSign(Column col)
    {
        var propTypeName = col.PropertyType;
        var result = "";
        if (col.IsNullable &&
            propTypeName != "byte[]" &&
            propTypeName != "string" &&
            propTypeName != "Microsoft.SqlServer.Types.SqlGeography" &&
            propTypeName != "Microsoft.SqlServer.Types.SqlGeometry"
           )
        {
            result = "?";
        }

        return result;
    }
}
