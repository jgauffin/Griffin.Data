using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding;
using MySql.Data.MySqlClient;

namespace Griffin.Data.MySql;

internal class MySqlSchemaReader : ISchemaReader
{
    private const string TableSql = @"
			SELECT * 
			FROM information_schema.TableCollection 
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
			";

    /// <inheritdoc />
    public async Task ReadSchema(IDbConnection connection, SchemaReaderContext context)
    {
        var tables = new List<Table>();

        if (connection is not MySqlConnection mySqlConnection)
        {
            throw new InvalidOperationException("The MySQL reader expected a MysqlConnection.");
        }

        var cmd = mySqlConnection.CreateCommand();
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        await using (cmd)
        {
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var tableName = rdr["TABLE_NAME"].ToString()!;
                var tbl = new Table(tableName)
                {
                    SchemaName = rdr["TABLE_SCHEMA"].ToString(),
                    IsView =
                        string.Equals(rdr["TABLE_TYPE"].ToString(), "View", StringComparison.OrdinalIgnoreCase)
                };

                context.Add(tbl);
                tables.Add(tbl);
            }
        }

        var schema = await mySqlConnection.GetSchemaAsync("COLUMNS");
        foreach (var item in tables)
        {
            item.Columns = new List<Column>();

            //pull the columns from the schema
            var columns = schema.Select("TABLE_NAME='" + item.Name + "'");
            foreach (var row in columns)
            {
                var dataType = (string)row["DATA_TYPE"];
                var propertyType = GetPropertyType(row);

                var col = new Column(row["COLUMN_NAME"].ToString()!, dataType, propertyType);
                col.PropertyName = col.Name.ToPropertyName();
                col.IsNullable = row["IS_NULLABLE"].ToString() == "YES";
                col.IsPrimaryKey = row["COLUMN_KEY"].ToString() == "PRI";
                col.IsAutoIncrement = row["extra"].ToString()!.ToLower()
                    .IndexOf("auto_increment", StringComparison.OrdinalIgnoreCase) >= 0;

                item.Columns.Add(col);
            }
        }
    }

    private static string GetPropertyType(DataRow row)
    {
        var bUnsigned = row["COLUMN_TYPE"].ToString()!.IndexOf("unsigned", StringComparison.OrdinalIgnoreCase) >= 0;
        var propType = "string";
        switch (row["DATA_TYPE"].ToString())
        {
            case "bigint":
                propType = bUnsigned ? "ulong" : "long";
                break;
            case "int":
                propType = bUnsigned ? "uint" : "int";
                break;
            case "smallint":
                propType = bUnsigned ? "ushort" : "short";
                break;
            case "guid":
                propType = "Guid";
                break;
            case "smalldatetime":
            case "date":
            case "datetime":
            case "timestamp":
                propType = "DateTime";
                break;
            case "float":
                propType = "float";
                break;
            case "double":
                propType = "double";
                break;
            case "numeric":
            case "smallmoney":
            case "decimal":
            case "money":
                propType = "decimal";
                break;
            case "bit":
            case "bool":
            case "boolean":
                propType = "bool";
                break;
            case "tinyint":
                propType = bUnsigned ? "byte" : "sbyte";
                break;
            case "image":
            case "binary":
            case "blob":
            case "mediumblob":
            case "longblob":
            case "varbinary":
                propType = "byte[]";
                break;
        }

        return propType;
    }
}
