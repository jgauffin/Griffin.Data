namespace Griffin.Data.Scaffolding.Queries.Meta;

//TODO: This is only for mssql
internal class SqlType
{
    public static Type ToDotNetType(string sqlType)
    {
        return sqlType switch
        {
            "bigint" => typeof(long),
            "binary" => typeof(byte[]),
            "image" => typeof(byte[]),
            "varbinary" => typeof(byte[]),
            "bit" => typeof(bool),
            "char" => typeof(string),
            "nchar" => typeof(string),
            "ntext" => typeof(string),
            "nvarchar" => typeof(string),
            "text" => typeof(string),
            "varchar" => typeof(string),
            "xml" => typeof(string),
            "date" => typeof(DateTime),
            "datetime" => typeof(DateTime),
            "datetime2" => typeof(DateTime),
            "smalldatetime" => typeof(DateTime),
            "decimal" => typeof(decimal),
            "money" => typeof(decimal),
            "numeric" => typeof(decimal),
            "smallmoney" => typeof(decimal),
            "float" => typeof(double),
            "int" => typeof(int),
            "real" => typeof(float),
            "smallint" => typeof(short),
            "time" => typeof(TimeSpan),
            "timestamp" => typeof(byte[]),
            "uniqueidentifier" => typeof(Guid),
            _ => throw new NotSupportedException("Cannot translate " + sqlType)
        };
    }
}
