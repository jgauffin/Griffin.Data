using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Meta;
using Griffin.Data.Scaffolding;

namespace Griffin.Data.SqlServer;

/// <summary>
///     TODO: IS this express now?
/// </summary>
[SchemaReader("SqlServerCE")]
internal class SqlServerCeSchemaReader : ISchemaReader
{
    private const string TableSql = @"SELECT *
		FROM  INFORMATION_SCHEMA.Tables
		WHERE TABLE_TYPE='TABLE'";

    private const string ColumnSql = @"SELECT 
			TABLE_CATALOG AS [Database],
			TABLE_SCHEMA AS Owner, 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS DefaultSetting, 
			IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
			AUTOINC_INCREMENT,
			CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
			DATETIME_PRECISION AS DatePrecision
		FROM  INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@tableName
		ORDER BY OrdinalPosition ASC";

    // SchemaReader.ReadSchema

    public async Task ReadSchema(SchemaReaderContext context)
    {
        var result = new List<Table>();
        await using var connection = new SqlConnection(context.ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        await using (cmd)
        {
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var name = rdr["TABLE_NAME"].ToString()!;
                var tbl = new Table(name) { ClassName = Inflector.Instance.MakeSingular(context.Cleanup(name)) };

                result.Add(tbl);
                context.Add(tbl);
            }
        }

        foreach (var tbl in result)
        {
            tbl.Columns = LoadColumns(connection, context, tbl);

            // Mark the primary key
            var primaryKey = GetPrimaryKey(connection, tbl.Name);
            var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
            if (pkColumn != null)
            {
                pkColumn.IsPrimaryKey = true;
            }
        }
    }

    private static string GetPrimaryKey(IDbConnection connection, string table)
    {
        var sql = @"SELECT KCU.COLUMN_NAME 
			FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
			JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
			ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME
			WHERE TC.CONSTRAINT_TYPE='PRIMARY KEY'
			AND KCU.TABLE_NAME=@tableName";

        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = table;
        cmd.Parameters.Add(p);

        var result = cmd.ExecuteScalar();
        return result?.ToString() ?? "";
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
            _ => "string"
        };
    }

    private static List<Column> LoadColumns(IDbConnection connection, SchemaReaderContext context, Table tbl)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = ColumnSql;

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = tbl.Name;
        cmd.Parameters.Add(p);

        var result = new List<Column>();
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var name = (string)rdr["ColumnName"];
            var propType = GetPropertyType((string)rdr["DataType"]);
            var col = new Column(name, "", propType)
            {
                PropertyName = Inflector.Instance.MakeSingular(context.Cleanup(name)),
                IsNullable = rdr["IsNullable"].ToString() == "YES",
                IsAutoIncrement = rdr["AUTOINC_INCREMENT"] != DBNull.Value
            };
            result.Add(col);
        }

        return result;
    }
}
