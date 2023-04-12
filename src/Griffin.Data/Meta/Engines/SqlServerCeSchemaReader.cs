using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml.Linq;

namespace Griffin.Data.Meta.Engines;

internal class SqlServerCeSchemaReader : SchemaReader
{
    private const string TableSql = @"SELECT *
		FROM  INFORMATION_SCHEMA.TableCollection
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
    public override TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory)
    {
        var result = new TableCollection();
        var cmd = connection.CreateCommand();
        cmd.Connection = connection;
        cmd.CommandText = TableSql;

        //pull the TableCollection in a reader
        using (cmd)
        {
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var name = rdr["TABLE_NAME"].ToString();
                    var tbl = new Table
                    {
                        Name = name,
                        Schema = null,
                        IsView = false,
                        CleanName = CleanUp(name),
                        ClassName = Inflector.Instance.MakeSingular(name)
                    };
                    result.Add(tbl);
                }
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
        return result != null ? result.ToString() : "";
    }

    private string GetPropertyType(string sqlType)
    {
        var sysType = "string";
        switch (sqlType)
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
            case "time":
                sysType = "DateTime";
                break;
            case "float":
                sysType = "double";
                break;
            case "real":
                sysType = "float";
                break;
            case "numeric":
            case "smallmoney":
            case "decimal":
            case "money":
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

        return sysType;
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
            var name= rdr["ColumnName"].ToString();

            var col = new Column
            {
                Name = name,
                PropertyName = CleanUp(name),
                PropertyType = GetPropertyType(rdr["DataType"].ToString()),
                IsNullable = rdr["IsNullable"].ToString() == "YES",
                IsAutoIncrement = rdr["AUTOINC_INCREMENT"] != DBNull.Value
            };
            result.Add(col);
        }

        return result;
    }
}
