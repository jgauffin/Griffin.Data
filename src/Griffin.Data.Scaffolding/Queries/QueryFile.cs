using Griffin.Data.Scaffolding.Queries.Parser;

namespace Griffin.Data.Scaffolding.Queries;

/// <summary>
///     A parsed query file (i.e. "YourQuery.query.sql").
/// </summary>
public class QueryFile
{
    public QueryFile(string directory, string filename, string query)
    {
        Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        Filename = filename ?? throw new ArgumentNullException(nameof(filename));
        Query = query ?? throw new ArgumentNullException(nameof(query));
    }

    /// <summary>
    ///     Directory that the query was located in.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    ///     File name of the file (excluding file extension).
    /// </summary>
    public string Filename { get; }

    /// <summary>
    ///     Parameters defined in the file.
    /// </summary>
    public IList<QueryParameter> Parameters { get; set; } = new List<QueryParameter>();

    /// <summary>
    ///     Actual SQL query.
    /// </summary>
    public string Query { get; }

    /// <summary>
    ///     Use paging.
    /// </summary>
    public bool UsePaging { get; set; }

    /// <summary>
    ///     Use sorting.
    /// </summary>
    public bool UseSorting { get; set; }
}
