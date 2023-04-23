using System;
using System.Collections.Generic;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Context for <see cref="ISchemaReader" /> implementations.
/// </summary>
public class SchemaReaderContext
{
    private readonly List<Table> _tables;

    /// <summary>
    /// </summary>
    /// <param name="tables">Collection to fill with meta data.</param>
    public SchemaReaderContext(List<Table> tables)
    {
        _tables = tables ?? throw new ArgumentNullException(nameof(tables));
    }

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
