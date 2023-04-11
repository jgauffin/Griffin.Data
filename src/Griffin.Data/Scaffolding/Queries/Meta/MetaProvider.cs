using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Griffin.Data.Meta;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Queries.Meta
{
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

            List<QueryMetaColumn> columns = new List<QueryMetaColumn>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var column = new QueryMetaColumn
                {
                    Name = reader.GetName(i),
                };
                
                var dataType = reader.GetDataTypeName(i);
                pos = dataType.IndexOf('(');
                if (pos != -1)
                {
                    var len = dataType[(pos + 1)..].TrimEnd(')');
                    column.StringLength = int.Parse(len);
                    dataType = dataType[..pos];
                }

                column.PropertyType = reader.GetFieldType(i);
                column.SqlDataType = dataType;
                columns.Add(column);
            }


            List<QueryMetaParameter> parameters = new List<QueryMetaParameter>();
            foreach (var parameter in queryFile.Parameters)
            {
                var p = new QueryMetaParameter { Name = parameter.Name, 
                    DefaultValue = parameter.TestValue,
                     
                };
                pos = parameter.SqlType.IndexOf('(');
                if (pos != -1)
                {
                    var len = parameter.SqlType[(pos + 1)..].TrimEnd(')');
                    p.PropertyType = SqlType.ToDotNetType(parameter.SqlType[..pos]);
                }
                else
                {
                    p.PropertyType = SqlType.ToDotNetType(parameter.SqlType[..pos]);
                }
                parameters.Add(p);
            }

            return new QueryMeta
            {
                Directory = queryFile.Directory,
                Columns = columns,
                Parameters = parameters,
                QueryName = queryName,
                SqlQuery = queryFile.Query
            };
        }
    }
}
