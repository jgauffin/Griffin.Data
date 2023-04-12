namespace Griffin.Data.Scaffolding.Queries.Meta;

/// <summary>
///     Meta data for a parsed query.
/// </summary>
public class QueryMeta
{
    public QueryMeta(string queryName, string sqlQuery)
    {
        QueryName = queryName ?? throw new ArgumentNullException(nameof(queryName));
        SqlQuery = sqlQuery ?? throw new ArgumentNullException(nameof(sqlQuery));
    }

    /// <summary>
    ///     columns in the result set.
    /// </summary>
    public IList<QueryMetaColumn> Columns { get; set; } = new List<QueryMetaColumn>();

    /// <summary>
    ///     Directory to generate the query in.
    /// </summary>
    public string Directory { get; set; } = "";

    /// <summary>
    ///     Namespace to put the query class in.
    /// </summary>
    public string Namespace { get; set; } = "";

    /// <summary>
    ///     Parameters in the query class.
    /// </summary>
    public IList<QueryMetaParameter> Parameters { get; set; } = new List<QueryMetaParameter>();

    /// <summary>
    ///     Name of query (will be the query class name).
    /// </summary>
    public string QueryName { get; }

    /// <summary>
    ///     query to execute.
    /// </summary>
    public string SqlQuery { get; }

    /// <summary>
    ///     Generated class should support paging.
    /// </summary>
    public bool UsePaging { get; set; }

    /// <summary>
    ///     Generated class should support sorting.
    /// </summary>
    public bool UseSorting { get; set; }
}
