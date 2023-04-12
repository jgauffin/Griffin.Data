using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Griffin.Data.Meta.Engines;

internal class PostGreSqlSchemaReader : SchemaReader
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

    // SchemaReader.ReadSchema
    public override TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory)
    {
        var result = new TableCollection();

        var cmd = connection.CreateCommand();
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        using (cmd)
        {
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var name = rdr["table_name"].ToString();
                var tbl = new Table
                {
                    Name = name,
                    Schema = rdr["table_schema"].ToString(),
                    IsView = string.Compare(rdr["table_type"].ToString(), "View", true) == 0,
                    CleanName = CleanUp(name),
                    ClassName = Inflector.Instance.MakeSingular(name)
                };
                result.Add(tbl);
            }
        }

        foreach (var tbl in result)
        {
            tbl.Columns = LoadColumns(connection, tbl);

            // Mark the primary key
            var primaryKey = GetPK(connection, tbl.Name);
            var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
            if (pkColumn != null)
            {
                pkColumn.IsPrimaryKey = true;
            }
        }

        return result;
    }

    private string GetPK(DbConnection connection, string table)
    {
        var sql = @"SELECT kcu.column_name 
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc
			ON kcu.constraint_name=tc.constraint_name
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

    private string GetPropertyType(string sqlType)
    {
        switch (sqlType)
        {
            case "int8":
            case "serial8":
                return "long";

            case "bool":
                return "bool";

            case "bytea	":
                return "byte[]";

            case "float8":
                return "double";

            case "int4":
            case "serial4":
                return "int";

            case "money	":
                return "decimal";

            case "numeric":
                return "decimal";

            case "float4":
                return "float";

            case "int2":
                return "short";

            case "time":
            case "timetz":
            case "timestamp":
            case "timestamptz":
            case "date":
                return "DateTime";

            default:
                return "string";
        }
    }

    private List<Column> LoadColumns(DbConnection connection, Table tbl)
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
            var name= rdr["column_name"].ToString();
            var col = new Column
            {
                Name = name,
                PropertyName = CleanUp(name),
                PropertyType = GetPropertyType(rdr["udt_name"].ToString()),
                IsNullable = rdr["is_nullable"].ToString() == "YES",
                IsAutoIncrement = rdr["column_default"].ToString().StartsWith("nextval(")
            };
            result.Add(col);
        }

        return result;
    }
}
