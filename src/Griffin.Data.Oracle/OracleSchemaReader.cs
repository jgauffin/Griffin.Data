using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding;
using Oracle.ManagedDataAccess.Client;

namespace Griffin.Data.Oracle;

/// <summary>
///     Schema reader for Oracle SQL.
/// </summary>
internal class OracleSchemaReader : ISchemaReader
{
    private const string TableSql = @"select TABLE_NAME from USER_TableCollection";

    private const string ColumnSql = @"select table_name TableName, 
 column_name ColumnName, 
 data_type DataType, 
 data_scale DataScale,
 nullable IsNullable
 from USER_TAB_COLS utc 
 where table_name = upper(:tableName)
 order by column_id";

    public async Task ReadSchema(IDbConnection connection, SchemaReaderContext context)
    {
        var result = new List<Table>();

        if (connection is not OracleConnection con)
        {
            throw new InvalidOperationException("The Oracle reader expected an OracleConnection.");
        }

        var cmd = con.CreateCommand();
        cmd.CommandText = TableSql;
        cmd.GetType().GetProperty("BindByName")!.SetValue(cmd, true, null);

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
                    IsView = string.Equals(rdr["TABLE_TYPE"].ToString(), "View",
                        StringComparison.OrdinalIgnoreCase)
                };

                result.Add(tbl);
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

    private static string GetPrimaryKey(OracleConnection connection, string table)
    {
        var sql = @"select column_name from USER_CONSTRAINTS uc
  inner join USER_CONS_COLUMNS ucc on uc.constraint_name = ucc.constraint_name
where uc.constraint_type = 'P'
and uc.table_name = upper(:tableName)
and ucc.position = 1";

        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.GetType().GetProperty("BindByName")!.SetValue(cmd, true, null);

        var p = cmd.CreateParameter();
        p.ParameterName = ":tableName";
        p.Value = table;
        cmd.Parameters.Add(p);

        var result = cmd.ExecuteScalar();

        return result?.ToString() ?? "";
    }

    private static string GetPropertyType(string sqlType, string dataScale)
    {
        var sysType = "string";
        switch (sqlType.ToLower())
        {
            case "bigint":
                sysType = "long";
                break;
            case "smallint":
                sysType = "short";
                break;
            case "int":
                sysType = "int";
                break;
            case "uniqueidentifier":
                sysType = "Guid";
                break;
            case "smalldatetime":
            case "datetime":
            case "date":
                sysType = "DateTime";
                break;
            case "float":
                sysType = "double";
                break;
            case "real":
            case "numeric":
            case "smallmoney":
            case "decimal":
            case "money":
            case "number":
                sysType = "decimal";
                break;
            case "tinyint":
                sysType = "byte";
                break;
            case "bit":
                sysType = "bool";
                break;
            case "image":
            case "binary":
            case "varbinary":
            case "timestamp":
                sysType = "byte[]";
                break;
        }

        if (sqlType == "number" && dataScale == "0")
        {
            return "long";
        }

        return sysType;
    }

    private List<Column> LoadColumns(OracleConnection connection, Table tbl)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = ColumnSql;
        var prop = cmd.GetType().GetProperty("BindByName");
        if (prop == null)
        {
            throw new InvalidOperationException("Expected to find a BindByName in the Oracle Command.");
        }

        prop.SetValue(cmd, true, null);

        var p = cmd.CreateParameter();
        p.ParameterName = ":tableName";
        p.Value = tbl.Name;
        cmd.Parameters.Add(p);

        var result = new List<Column>();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var name = (string)rdr["ColumnName"];
            var dataType = rdr["DataType"] == DBNull.Value ? null : (string)rdr["DataType"];
            var propType = GetPropertyType(rdr["DataType"].ToString()!, (string)rdr["DataScale"]);
            var col = new Column(name, dataType, propType)
            {
                PropertyName = name.ToPropertyName(),
                IsNullable = rdr["IsNullable"].ToString() == "YES",
                IsAutoIncrement = true
            };
            result.Add(col);
        }

        return result;
    }
}
