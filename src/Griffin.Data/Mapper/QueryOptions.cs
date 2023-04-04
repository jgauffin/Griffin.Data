using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Griffin.Data.Helpers;

namespace Griffin.Data.Mapper;

/// <summary>
///     Factory delegate for <see cref="QueryOptions" />.
/// </summary>
/// <param name="record">Data record used to determine which type of entity to load.</param>
/// <returns>Created entity.</returns>
public delegate object CreateEntityDelegate(IDataRecord record);

public class QueryOptions<T>
{
    /// <summary>
    /// </summary>
    /// <param name="sql">SQL query (complete or partial, refer to the wiki for more information).</param>
    /// <param name="constraints">
    ///     Keys must match the names in the SQL statement.
    /// </param>
    public QueryOptions(string sql, object constraints)
    {
        Options.Sql = sql;
        Options.Parameters = constraints.ToDictionary();
    }

    /// <summary>
    /// </summary>
    /// <param name="constraints">
    ///     Keys must match the names in the SQL statement.
    /// </param>
    public QueryOptions(object constraints)
    {
        Options.Parameters = constraints.ToDictionary();
    }

    public QueryOptions()
    {
    }

    internal QueryOptions Options { get; } = new();


    public QueryOptions<T> Where(object propertyConstraints)
    {
        Options.Parameters = propertyConstraints.ToDictionary();
        return this;
    }

    public QueryOptions<T> Where(string sql, object columnConstraints)
    {
        Options.Sql = sql;
        Options.DbParameters = columnConstraints.ToDictionary();
        return this;
    }

    public QueryOptions<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> property)
    {
        Options.OrderBy(property.GetMemberName());
        return this;
    }

    public QueryOptions<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> property)
    {
        Options.OrderByDescending(property.GetMemberName());
        return this;
    }

    public QueryOptions<T> Paging(int pageNumber, int pageSize)
    {
        Options.PageSize = pageSize;
        Options.PageNumber = pageNumber;
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
    ///     Load all child entities.
    /// </summary>
    /// <value>
    ///     Default is <c>true</c>.
    /// </value>
    public bool LoadChildren { get; set; } = true;

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

    /// <summary>
    ///     Constraints used (property names are used).
    /// </summary>
    public IDictionary<string, object>? Parameters { get; set; }

    /// <summary>
    ///     SQL parameters (i.e. column names are used).
    /// </summary>
    public IDictionary<string, object>? DbParameters { get; set; }

    /// <summary>
    ///     Factory used to create the correct entity.
    /// </summary>
    public CreateEntityDelegate? Factory { get; set; }

    /// <summary>
    ///     Number of items per page (used as row limit when paging is not used).
    /// </summary>
    /// <value>Default is 100</value>
    public int PageSize { get; set; } = 100;

    /// <summary>
    ///     Which page to fetch, one-based index.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    internal List<SortInstruction> Sorts { get; set; } = new List<SortInstruction>();

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

    public static QueryOptions<T> Where<T>(string sql, object parameters)
    {
        return new QueryOptions<T>(sql, parameters);
    }

    public static QueryOptions<T> Where<T>(object parameters)
    {
        return new QueryOptions<T>(parameters);
    }
}

internal class SortInstruction
{
    public SortInstruction(string name, bool isAscending, bool isPropertyName)
    {
        Name = name;
        IsAscending = isAscending;
        IsPropertyName = isPropertyName;
    }

    public string Name { get; set; }
    public bool IsAscending { get; set; }
    public bool IsPropertyName { get; }
}