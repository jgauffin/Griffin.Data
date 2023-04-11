using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper.Helpers;

namespace Griffin.Data.Mapper;

/// <summary>
///     Query options.
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueryOptions<T>
{
    /// <summary>
    /// </summary>
    /// <param name="sql">SQL query (complete or partial, refer to the wiki for more information).</param>
    /// <param name="constraints">
    ///     Keys must match the names in the SQL statement.
    /// </param>
    public QueryOptions(Session session, string sql, object constraints)
    {
        Session = session;

        Options.Sql = sql;
        Options.DbParameters = constraints.ToDictionary();
    }
    
    /// <summary>
    /// </summary>
    /// <param name="constraints">
    ///     Keys must match the names in the SQL statement.
    /// </param>
    public QueryOptions(Session session, object constraints)
    {
        Session = session;
        Options.Parameters = constraints.ToDictionary();
    }

    public QueryOptions(Session session)
    {
        Session = session;
    }

    internal QueryOptions Options { get; } = new();

    internal Session Session { get; }

    /// <summary>
    ///     Do not load any children.
    /// </summary>
    /// <returns>this.</returns>
    public QueryOptions<T> DoNotLoadChildren()
    {
        Options.LoadChildren = false;
        return this;
    }

    /// <summary>
    ///     IsAscending sort.
    /// </summary>
    /// <typeparam name="TProperty">Property to sort after.</typeparam>
    /// <param name="property">Property selector</param>
    /// <returns>this.</returns>
    public QueryOptions<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        Options.OrderBy(property.GetMemberName());
        return this;
    }

    /// <summary>
    ///     Descending sort.
    /// </summary>
    /// <typeparam name="TProperty">Property to sort after.</typeparam>
    /// <param name="property">Property selector</param>
    /// <returns>this.</returns>
    public QueryOptions<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        Options.OrderByDescending(property.GetMemberName());
        return this;
    }

    /// <summary>
    ///     Page result.
    /// </summary>
    /// <param name="pageNumber">One based index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns></returns>
    public QueryOptions<T> Paging(int pageNumber, int pageSize)
    {
        Options.PageSize = pageSize;
        Options.PageNumber = pageNumber;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="propertyConstraints"></param>
    /// <returns></returns>
    public QueryOptions<T> Where(object propertyConstraints)
    {
        if (propertyConstraints == null)
        {
            throw new ArgumentNullException(nameof(propertyConstraints));
        }

        Options.Parameters = propertyConstraints.ToDictionary();
        return this;
    }

    /// <summary>
    ///     Create a complete SQL statement or a WHERE statement.
    /// </summary>
    /// <param name="sql">Complete SQL or short form.</param>
    /// <param name="parameters">Parameters for the SQL statement.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentNullException">any of the arguments are <c>null</c>.</exception>
    public QueryOptions<T> Where(string sql, object parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        Options.Sql = sql ?? throw new ArgumentNullException(nameof(sql));
        Options.DbParameters = parameters.ToDictionary();
        return this;
    }
}

/// <summary>
///     Options used to fine tune how a query should be executed.
/// </summary>
public class QueryOptions
{
    /// <summary>
    /// </summary>
    /// <param name="sql">SQL query (complete or partial, refer to the wiki for more information).</param>
    /// <param name="constraints">
    ///     Parameters used in the query. Can either be used stand alone or to specify the parameters
    ///     used in the SQL query.
    /// </param>
    public QueryOptions(string? sql, object? constraints)
    {
        Sql = sql;
        Parameters = constraints?.ToDictionary();
    }

    public QueryOptions()
    {
    }

    /// <summary>
    ///     SQL parameters (i.e. column names are used).
    /// </summary>
    public IDictionary<string, object>? DbParameters { get; set; }

    /// <summary>
    ///     Factory used to create the correct entity.
    /// </summary>
    public CreateEntityDelegate? Factory { get; set; }

    /// <summary>
    ///     Load all child entities.
    /// </summary>
    /// <value>
    ///     Default is <c>true</c>.
    /// </value>
    public bool LoadChildren { get; set; } = true;

    /// <summary>
    ///     Which page to fetch, one-based index.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    ///     Number of items per page (used as row limit when paging is not used).
    /// </summary>
    /// <value>Default is 100</value>
    public int PageSize { get; set; } = 100;

    /// <summary>
    ///     Constraints used (property names are used).
    /// </summary>
    public IDictionary<string, object>? Parameters { get; set; }

    /// <summary>
    ///     SQL query to use.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A normal SELECT query will be generated with <see cref="Parameters" /> and <see cref="DbParameters" /> if this
    ///         is not specified.
    ///     </para>
    /// </remarks>
    public string? Sql { get; set; }

    internal List<SortInstruction> Sorts { get; set; } = new();

    public void OrderBy(string name, bool isPropertyName = true)
    {
        Sorts.Add(new SortInstruction(name, true, isPropertyName));
    }

    public void OrderByDescending(string name, bool isPropertyName = true)
    {
        Sorts.Add(new SortInstruction(name, false, isPropertyName));
    }

    public static QueryOptions Where(object propertyConstraints)
    {
        return new QueryOptions("", propertyConstraints);
    }
}
