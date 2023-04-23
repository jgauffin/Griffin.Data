using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.SqlServer;

internal class SqlServerSchemaReader : ISchemaReader
{
    // SchemaReader.ReadSchema

    private const string TableSql = @"SELECT *
		FROM  INFORMATION_SCHEMA.Tables
		WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'";

    public async Task ReadSchema(IDbConnection connection, SchemaReaderContext context)
    {
        var result = new List<Table>();

        if (connection is not SqlConnection con)
        {
            throw new InvalidOperationException("The SqlServer reader expected a SqlConnection.");
        }

        var cmd = con.CreateCommand();
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        await using (cmd)
        {
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var name = (string)rdr["TABLE_NAME"];
                var tbl = new Table(name)
                {
                    SchemaName = rdr["TABLE_SCHEMA"].ToString(),
                    IsView = string.Compare(rdr["TABLE_TYPE"].ToString(), "View",
                        StringComparison.OrdinalIgnoreCase) == 0
                };

                result.Add(tbl);
                context.Add(tbl);
            }
        }

        foreach (var tbl in result)
        {
            tbl.Columns = LoadColumns(con, tbl);
        }

        var tables = result.ToDictionary(x => x.Name, x => x);
        await AddForeignKeys(tables, connection);
        await SetPrimaryKeys(con, tables);
    }

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
            var foreignKeyTable = reader.GetString(1);
            var foreignKeyColumn = reader.GetString(2);
            var referencedTable = reader.GetString(3);
            var referencedColumn = reader.GetString(4);

            // do not make references as keys.
            if (foreignKeyColumn.StartsWith("created", StringComparison.OrdinalIgnoreCase) ||
                foreignKeyColumn.StartsWith("updated", StringComparison.OrdinalIgnoreCase) ||
                foreignKeyColumn.Contains("user", StringComparison.OrdinalIgnoreCase) ||
                foreignKeyColumn.Contains("account", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!tables.TryGetValue(foreignKeyTable, out var foreignKeyMeta))
            {
                continue;
            }

            if (!tables.TryGetValue(referencedTable, out var referencedMeta))
            {
                continue;
            }

            foreignKeyMeta.ForeignKeys.Add(new ForeignKeyColumn(foreignKeyColumn, referencedMeta, referencedColumn));
            referencedMeta.References.Add(new Reference(referencedColumn, foreignKeyMeta, foreignKeyColumn));
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
                referencedTable?.References.Add(new Reference("Id", table, column.PropertyName));
            }
        }
    }

    private static string GetPropertyType(string sqlType)
    {
        return sqlType switch
        {
            "bigint" => "long",
            "smallint" => "short",
            "int" => "int",
            "uniqueidentifier" => "Guid",
            "smalldatetime" => "DateTime",
            "datetime" => "DateTime",
            "date" => "DateTime",
            "time" => "DateTime",
            "float" => "double",
            "real" => "float",
            "numeric" => "decimal",
            "smallmoney" => "decimal",
            "decimal" => "decimal",
            "money" => "decimal",
            "tinyint" => "byte",
            "bit" => "bool",
            "image" => "byte[]",
            "binary" => "byte[]",
            "varbinary" => "byte[]",
            "timestamp" => "byte[]",
            "geography" => "Microsoft.SqlServer.Types.SqlGeography",
            "geometry" => "Microsoft.SqlServer.Types.SqlGeometry",
            _ => "string"
        };
    }

    private static List<Column> LoadColumns(SqlConnection connection, Table tbl)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT 
			TABLE_CATALOG AS [Database],
			TABLE_SCHEMA AS Owner, 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS DefaultSetting, 
			IS_NULLABLE AS IsNullable, 
            DATA_TYPE AS DataType, 
			CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
			DATETIME_PRECISION AS DatePrecision,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
		FROM  INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
		ORDER BY OrdinalPosition ASC";
        ;

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = tbl.Name;
        cmd.Parameters.Add(p);

        p = cmd.CreateParameter();
        p.ParameterName = "@schemaName";
        p.Value = tbl.SchemaName;
        cmd.Parameters.Add(p);

        var result = new List<Column>();
        using IDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var name = reader["ColumnName"].ToString()!;
            var dataType = reader["DataType"].ToString()!;
            var propertyType = GetPropertyType(dataType);

            var col = new Column(name, dataType, propertyType)
            {
                MaxStringLength = reader.GetNullableInt("MaxLength"),
                DefaultValue = reader.GetNullableString("DefaultSetting"),
                PropertyName = name.ToPropertyName(),
                IsNullable = reader.GetNullableString("IsNullable") == "YES",
                IsAutoIncrement = (int)reader["IsIdentity"] == 1
            };
            result.Add(col);
        }

        return result;
    }

    private static async Task SetPrimaryKeys(SqlConnection connection, IDictionary<string, Table> tables)
    {
        var sql = @"SELECT c.name AS ColumnName, o.Name as tableName, c.is_identity
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.type = 1)";

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(1);
            if (!tables.TryGetValue(tableName, out var table))
            {
                continue;
            }

            var columnName = reader.GetString(0);
            if (columnName == null)
            {
                continue;
            }

            var column = table.Columns.FirstOrDefault(x => x.Name == columnName);
            if (column != null)
            {
                column.IsPrimaryKey = true;
                if (reader.GetBoolean(2))
                {
                    column.IsAutoIncrement = true;
                }
            }
        }

        foreach (var value in tables.Values)
        {
            if (!value.Columns.Any(x => x.IsPrimaryKey))
            {
                throw new InvalidOperationException(
                    $"Table '{value.Name}' does not have a primary key defined, which is a requirement.");
            }
        }
    }
}
