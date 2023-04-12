using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.Scaffolding.Meta;

public class TableSchemaReader
{
    public async Task AddForeignKeys(IDictionary<string, Table> tables, IDbConnection connection)
    {
        var sql =
            @"SELECT fk.name, OBJECT_NAME(fk.parent_object_id) 'ParentTable', c1.name 'ParentColumn', OBJECT_NAME(fk.referenced_object_id) 'ReferencedTable', c2.name 'ReferencedColumn'
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
INNER JOIN sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id
INNER JOIN sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id";

        await using var cmd = (DbCommand)connection.CreateCommand();
        cmd.CommandText = sql;
        await using var reader = await cmd.ExecuteReaderAsync();
        var found = false;
        while (await reader.ReadAsync())
        {
            found = true;
            var parentTable = reader.GetString(1);
            var foreignKeyColumn = reader.GetString(2);
            var referencedTable = reader.GetString(3);
            var referencedColumn = reader.GetString(4);
            if (!tables.TryGetValue(parentTable, out var p))
            {
                continue;
            }

            if (!tables.TryGetValue(referencedTable, out var r))
            {
                continue;
            }

            p.ForeignKeys.Add(new ForeignKeyColumn(foreignKeyColumn, p, referencedColumn));
            r.References.Add(new Reference(referencedColumn, p, foreignKeyColumn));
        }

        if (!found)
        {
            foreach (var table in tables.Values)
            foreach (var column in table.Columns)
            {
                if (!column.PropertyName.EndsWith("Id"))
                {
                    continue;
                }

                var className = column.PropertyName.Replace("Id", "");
                var referencedTable = tables.Values.FirstOrDefault(x => x.ClassName == className);
                if (referencedTable != null)
                {
                    referencedTable.References.Add(new Reference("Id", table, column.PropertyName));
                }
            }
        }
    }

    public async Task<IReadOnlyList<Table>> Generate(IDbConnection connection)
    {
        var sql =
            @"SELECT  c.TABLE_NAME, c.COLUMN_NAME,c.DATA_TYPE, c.Column_default, c.character_maximum_length, c.numeric_precision, c.is_nullable
             ,CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PRIMARY KEY' ELSE '' END AS KeyType
FROM INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN (
            SELECT ku.TABLE_CATALOG,ku.TABLE_SCHEMA,ku.TABLE_NAME,ku.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku
                ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
         )   pk 
ON  c.TABLE_CATALOG = pk.TABLE_CATALOG
            AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA
            AND c.TABLE_NAME = pk.TABLE_NAME
            AND c.COLUMN_NAME = pk.COLUMN_NAME
ORDER BY c.TABLE_SCHEMA,c.TABLE_NAME, c.ORDINAL_POSITION 
";
        var tables = new Dictionary<string, Table>();

        await using (var cmd = (DbCommand)connection.CreateCommand())
        {
            cmd.CommandText = sql;
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                //TableName, ColumnName, Datatype, DefaultValue, MaxStringLength, NumericPrecision, Nullable, KeyType
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    if (!tables.TryGetValue(tableName, out var table))
                    {
                        table = new Table(tableName);
                        tables[tableName] = table;
                    }

                    var sqlType = reader.GetString(2);

                    var column = new Column(reader.GetString(1), sqlType, SqlType.ToDotNetType(sqlType))
                    {
                        DefaultValue = reader.GetNullableString(3),
                        MaxStringLength = reader.GetNullableInt(4),
                        IsNullable = reader.GetString(6) == "YES",
                        IsPrimaryKey = reader.GetString(7) == "PRIMARY KEY"
                    };
                    column.PropertyName = column.ColumnName.ToPascalCase();
                    table.Columns.Add(column);
                }
            }
        }

        await AddForeignKeys(tables, connection);
        var namespaces = IdentityNamespaces(tables);
        foreach (var ns in namespaces)
        {
            var nsTables = tables.Values.Where(x => x.ClassName.StartsWith(ns) && x.Namespace == "");
            foreach (var nsTable in nsTables)
            {
                nsTable.Namespace = ns.Pluralize();
            }
        }

        return tables.Values.ToList();
    }

    private IReadOnlyList<string> IdentityNamespaces(Dictionary<string, Table> tables)
    {
        var names = tables.Keys.ToArray();
        var namespaces = new List<string>();
        for (var i = 0; i < names.Length; i++)
        {
            var word = names[i];
            while (true)
            {
                word = ReduceWordString(word);
                if (word == "")
                {
                    break;
                }

                for (var j = i + 1; j < names.Length; j++)
                {
                    if (names[j].StartsWith(word) && !namespaces.Contains(word))
                    {
                        namespaces.Add(word);
                    }
                }
            }
        }

        return namespaces.OrderByDescending(x => x.Length).ToList();
    }

    private string ReduceWordString(string name)
    {
        for (var i = name.Length - 1; i > 0; i--)
        {
            if (char.IsUpper(name[i]))
            {
                return name.Substring(0, i);
            }
        }

        return "";
    }

   
}
