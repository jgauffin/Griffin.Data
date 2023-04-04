using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Griffin.Data.Meta.Engines;

internal class SqlServerSchemaReader : SchemaReader
{
    // SchemaReader.ReadSchema

    private const string TABLE_SQL = @"SELECT *
		FROM  INFORMATION_SCHEMA.TableCollection
		WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'";

    private const string COLUMN_SQL = @"SELECT 
			TABLE_CATALOG AS [Database],
			TABLE_SCHEMA AS Owner, 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS DefaultSetting, 
			IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
			CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
			DATETIME_PRECISION AS DatePrecision,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
		FROM  INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
		ORDER BY OrdinalPosition ASC";

    private DbConnection _connection;
    private DbProviderFactory _factory;

    public override TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory)
    {
        var result = new TableCollection();

        _connection = connection;
        _factory = factory;

        var cmd = _factory.CreateCommand();
        cmd.Connection = connection;
        cmd.CommandText = TABLE_SQL;

        //pull the TableCollection in a reader
        using (cmd)
        {
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var tbl = new Table();
                    tbl.Name = rdr["TABLE_NAME"].ToString();
                    tbl.Schema = rdr["TABLE_SCHEMA"].ToString();
                    tbl.IsView =
                        string.Compare(rdr["TABLE_TYPE"].ToString(), "View", StringComparison.OrdinalIgnoreCase) ==
                        0;
                    tbl.CleanName = CleanUp(tbl.Name);
                    tbl.ClassName = Inflector.Instance.MakeSingular(tbl.CleanName);

                    result.Add(tbl);
                }
            }
        }

        foreach (var tbl in result)
        {
            tbl.Columns = LoadColumns(tbl);

            // Mark the primary key
            var primaryKey = GetPrimaryKey(tbl.Name);
            var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
            if (pkColumn != null) pkColumn.IsPrimaryKey = true;
        }


        return result;
    }


    private List<Column> LoadColumns(Table tbl)
    {
        using (var cmd = _factory.CreateCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandText = COLUMN_SQL;

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = tbl.Name;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = "@schemaName";
            p.Value = tbl.Schema;
            cmd.Parameters.Add(p);

            var result = new List<Column>();
            using (IDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var col = new Column();
                    col.Name = rdr["ColumnName"].ToString();
                    col.PropertyName = CleanUp(col.Name);
                    col.PropertyType = GetPropertyType(rdr["DataType"].ToString());
                    col.IsNullable = rdr["IsNullable"].ToString() == "YES";
                    col.IsAutoIncrement = (int)rdr["IsIdentity"] == 1;
                    result.Add(col);
                }
            }

            return result;
        }
    }

    private string GetPrimaryKey(string table)
    {
        var sql = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.type = 1) AND (o.name = @tableName)";

        using (var cmd = _factory.CreateCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandText = sql;

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = table;
            cmd.Parameters.Add(p);

            var result = cmd.ExecuteScalar();

            if (result != null)
                return result.ToString();
        }

        return "";
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
            case "geography":
                sysType = "Microsoft.SqlServer.Types.SqlGeography";
                break;
            case "geometry":
                sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                break;
        }

        return sysType;
    }
}