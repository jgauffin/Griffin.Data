using System;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Defines which DB engine a reader is for.
/// </summary>
public class SchemaReaderAttribute : Attribute
{
    /// <summary>
    /// </summary>
    /// <param name="databaseEngineName">Name used (and displayed to the user) when selecting which DB to generate for.</param>
    /// <exception cref="ArgumentNullException">Name is null.</exception>
    public SchemaReaderAttribute(string databaseEngineName)
    {
        DatabaseEngineName = databaseEngineName ?? throw new ArgumentNullException(nameof(databaseEngineName));
    }

    /// <summary>
    ///     Name used (and displayed to the user) when selecting which DB to generate for.
    /// </summary>
    public string DatabaseEngineName { get; }
}
