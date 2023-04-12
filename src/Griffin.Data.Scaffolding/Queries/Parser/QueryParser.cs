using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Griffin.Data.Scaffolding.Queries.Parser;

internal class QueryParser
{
    private readonly List<QueryParameter> _parameters = new();
    private readonly StringBuilder _query = new();
    private bool _usePaging;
    private bool _useSorting;

    public QueryFile ParseFile(string fullPath, string sql)
    {
        var reader = new StringReader(sql);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line == "--paging")
            {
                _usePaging = true;
                continue;
            }

            if (line == "--sorting")
            {
                _useSorting = true;
                continue;
            }

            if (line.Trim().StartsWith("declare"))
            {
                var parser = new ParameterParser();
                var parameter = parser.ParseParameter(line);
                _parameters.Add(parameter);
                continue;
            }

            _query.AppendLine(line);
        }

        return new QueryFile(Path.GetDirectoryName(fullPath)!, Path.GetFileNameWithoutExtension(fullPath), _query.ToString())
        {
            Parameters = _parameters,
            UsePaging = _usePaging,
            UseSorting = _useSorting
        };
    }
    
}
