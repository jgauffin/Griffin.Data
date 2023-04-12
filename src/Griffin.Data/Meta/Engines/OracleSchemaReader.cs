using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Griffin.Data.Meta.Engines;

/// <summary>
///     Schema reader for Oracle SQL.
/// </summary>
internal class OracleSchemaReader : SchemaReader
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

    public override TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory)
    {
        var result = new TableCollection();

        var cmd = connection.CreateCommand();
        cmd.Connection = connection;
        cmd.CommandText = TableSql;
        cmd.GetType().GetProperty("BindByName")!.SetValue(cmd, true, null);

        //pull the TableCollection in a reader
        using (cmd)
        {
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var name = rdr["TABLE_NAME"].ToString();
                var tbl = new Table
                {
                    Name = name,
                    Schema = rdr["TABLE_SCHEMA"].ToString(),
                    IsView = string.Compare(rdr["TABLE_TYPE"].ToString(), "View",
                                 StringComparison.OrdinalIgnoreCase) ==
                             0,
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
            var primaryKey = GetPrimaryKey(connection, tbl.Name);
            var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
            if (pkColumn != null)
            {
                pkColumn.IsPrimaryKey = true;
            }
        }

        return result;
    }

    private string GetPrimaryKey(DbConnection connection, string table)
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

        if (result != null)
        {
            return result.ToString();
        }

        return "";
    }

    private string GetPropertyType(string sqlType, string dataScale)
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

    private List<Column> LoadColumns(DbConnection connection, Table tbl)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = ColumnSql;
        cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);

        var p = cmd.CreateParameter();
        p.ParameterName = ":tableName";
        p.Value = tbl.Name;
        cmd.Parameters.Add(p);

        var result = new List<Column>();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var name= rdr["ColumnName"].ToString();

            var col = new Column
            {
                Name = name,
                PropertyName = CleanUp(name),
                PropertyType = GetPropertyType(rdr["DataType"].ToString(),
                    rdr["DataType"] == DBNull.Value ? null : rdr["DataType"].ToString()),
                IsNullable = rdr["IsNullable"].ToString() == "YES",
                IsAutoIncrement = true
            };
            result.Add(col);
        }

        return result;
    }
}
