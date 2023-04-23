using System;
using Griffin.Data.Dialects;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Assigns a database engine name to a type (typically a <see cref="ISqlDialect" /> implementation).
/// </summary>
public class DbEngineNameAttribute : Attribute
{
    /// <summary>
    /// </summary>
    /// <param name="databaseEngineName">Name used (and displayed to the user) when selecting which DB to generate for.</param>
    /// <exception cref="ArgumentNullException">Name is null.</exception>
    public DbEngineNameAttribute(string databaseEngineName)
    {
        DatabaseEngineName = databaseEngineName ?? throw new ArgumentNullException(nameof(databaseEngineName));
    }

    /// <summary>
    ///     Name used (and displayed to the user) when selecting which DB to generate for.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The name should consist of one word and an well known alias for the DB engine. Like "mssql" for Microsoft SQL
    ///         server.
    ///     </para>
    /// </remarks>
    public string DatabaseEngineName { get; }
}
