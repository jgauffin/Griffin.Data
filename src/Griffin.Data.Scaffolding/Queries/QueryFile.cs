using System.Collections.Generic;
using Griffin.Data.Scaffolding.Queries.Parser;

namespace Griffin.Data.Scaffolding.Queries;

public class QueryFile
{
    public string Filename { get; set; }
    public bool UsePaging { get; set; }
    public bool UseSorting { get; set; }
    public IList<QueryParameter> Parameters { get; set; }
    public string Query { get; set; }
    public string Directory { get; set; }
}
