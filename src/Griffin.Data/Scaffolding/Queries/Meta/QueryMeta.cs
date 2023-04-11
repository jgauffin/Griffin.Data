using System.Collections.Generic;

namespace Griffin.Data.Scaffolding.Queries.Meta;

public class QueryMeta
{
    public string Namespace { get; set; } = "";
    public string QueryName { get; set; }
    public string SqlQuery { get; set; }
    public bool UseSorting { get; set; }
    public bool UserPaging { get; set; }
    public IList<QueryMetaColumn> Columns { get; set; }
    public IList<QueryMetaParameter> Parameters { get; set; }
    public string Directory { get; set; }
}
