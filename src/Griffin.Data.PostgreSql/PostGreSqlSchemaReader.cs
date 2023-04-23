using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding;
using Npgsql;

namespace Griffin.Data.PostgreSql;

internal class PostGreSqlSchemaReader : ISchemaReader
{
    private const string TableSql = @"
			SELECT table_name, table_schema, table_type
			FROM information_schema.TableCollection 
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
				AND table_schema NOT IN ('pg_catalog', 'information_schema');
			";

    private const string ColumnSql = @"
			SELECT column_name, is_nullable, udt_name, column_default
			FROM information_schema.columns 
			WHERE table_name=@tableName;
			";

    /// <inheritdoc />
    public async Task ReadSchema(IDbConnection connection, SchemaReaderContext context)
    {
        var result = new List<Table>();

        if (connection is not NpgsqlConnection con)
        {
            throw new InvalidOperationException("The PostgreSql reader expected a NpgsqlConnection.");
        }

        var cmd = con.CreateCommand();
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        await using (cmd)
        {
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var name = rdr["table_name"].ToString()!;
                var tbl = new Table(name)
                {
                    SchemaName = rdr["table_schema"].ToString(),
                    IsView = string.Equals(rdr["table_type"].ToString(), "View",
                        StringComparison.OrdinalIgnoreCase)
                };

                result.Add(tbl);
                context.Add(tbl);
            }
        }

        foreach (var tbl in result)
        {
            tbl.Columns = LoadColumns(con, tbl);

            // Mark the primary key
            var primaryKey = GetPrimaryKey(con, tbl.Name);
            var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
            if (pkColumn != null)
            {
                pkColumn.IsPrimaryKey = true;
            }
        }
    }

    private static string GetPrimaryKey(NpgsqlConnection connection, string table)
    {
        var sql = @"SELECT kcu.column_name 
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc ON kcu.constraint_name=tc.constraint_name
			WHERE lower(tc.constraint_type)='primary key'
			AND kcu.table_name=@tablename";

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
            "int8" => "long",
            "serial8" => "long",
            "bool" => "bool",
            "bytea	" => "byte[]",
            "float8" => "double",
            "int4" => "int",
            "serial4" => "int",
            "money	" => "decimal",
            "numeric" => "decimal",
            "float4" => "float",
            "int2" => "short",
            "time" => "DateTime",
            "timetz" => "DateTime",
            "timestamp" => "DateTime",
            "timestamptz" => "DateTime",
            "date" => "DateTime",
            _ => "string"
        };
    }

    private static List<Column> LoadColumns(NpgsqlConnection connection, Table tbl)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = ColumnSql;

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = tbl.Name;
        cmd.Parameters.Add(p);

        var result = new List<Column>();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var name = rdr["column_name"].ToString()!;
            var propertyType = GetPropertyType(rdr["udt_name"].ToString()!);
            var col = new Column(name, "", propertyType)
            {
                PropertyName = name,
                IsNullable = rdr["is_nullable"].ToString() == "YES",
                IsAutoIncrement =
                    rdr["column_default"].ToString()!.StartsWith("nextval(", StringComparison.OrdinalIgnoreCase)
            };
            result.Add(col);
        }

        return result;
    }
}
