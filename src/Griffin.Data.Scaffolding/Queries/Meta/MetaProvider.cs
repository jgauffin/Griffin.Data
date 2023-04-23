using System.Data;
using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding.Queries.Meta;

internal class MetaProvider
{
    public QueryMeta GenerateMeta(QueryFile queryFile, IDbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = queryFile.Query;

        foreach (var parameter in queryFile.Parameters)
        {
            command.AddParameter(parameter.Name, parameter.TestValue);
        }

        using var reader = command.ExecuteReader();
        var pos = queryFile.Filename.IndexOf('.');
        var queryName = queryFile.Filename[..pos];

        var columns = new List<QueryMetaColumn>();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var column = new QueryMetaColumn(reader.GetName(i), reader.GetFieldType(i));

            var dataType = reader.GetDataTypeName(i);
            pos = dataType.IndexOf('(');
            if (pos != -1)
            {
                var len = dataType[(pos + 1)..].TrimEnd(')');
                column.StringLength = int.Parse(len);
                dataType = dataType[..pos];
            }

            column.SqlDataType = dataType;
            columns.Add(column);
        }

        var parameters = new List<QueryMetaParameter>();
        foreach (var parameter in queryFile.Parameters)
        {
            Type propertyType;
            pos = parameter.SqlType.IndexOf('(');
            if (pos != -1)
            {
                var len = parameter.SqlType[(pos + 1)..].TrimEnd(')');
                propertyType = SqlType.ToDotNetType(parameter.SqlType[..pos]);
            }
            else
            {
                propertyType = SqlType.ToDotNetType(parameter.SqlType);
            }

            var p = new QueryMetaParameter(parameter.Name, propertyType) { DefaultValue = parameter.TestValue };

            parameters.Add(p);
        }

        return new QueryMeta(queryName, queryFile.Query)
        {
            Directory = queryFile.Directory,
            Columns = columns,
            Parameters = parameters,
            UseSorting = queryFile.UseSorting,
            UsePaging = queryFile.UsePaging
        };
    }
}
