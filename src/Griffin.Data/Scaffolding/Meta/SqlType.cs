using System;

namespace Griffin.Data.Scaffolding.Meta;

public class SqlType
{
    public static Type ToDotNetType(string sqlType)
    {
        switch (sqlType)
        {
            case "bigint":
                return typeof(long);
            case "binary":
            case "image":
            case "varbinary":
                return typeof(byte[]);
            case "bit":
                return typeof(bool);
            case "char":
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "text":
            case "varchar":
            case "xml":
                return typeof(string);
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                return typeof(DateTime);
            case "decimal":
            case "money":
            case "numeric":
            case "smallmoney":
                return typeof(decimal);
            case "float":
                return typeof(double);
            case "int":
                return typeof(int);
            case "real":
                return typeof(float);
            case "smallint":
                return typeof(short);
            case "time":
                return typeof(TimeSpan);
            case "timestamp":
                return typeof(byte[]);
            case "uniqueidentifier":
                return typeof(Guid);
            default:
                throw new NotSupportedException("Cannot translate " + sqlType);
        }
    }
}
