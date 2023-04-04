using System;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Configuration;

/// <summary>
///     Defines a key configurator
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
/// <typeparam name="TProperty">Property the for this key.</typeparam>
public class KeyConfigurator<TEntity, TProperty>
{
    private readonly KeyMapping _mapping;

    /// <summary>
    /// </summary>
    /// <param name="mapping"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public KeyConfigurator(KeyMapping mapping)
    {
        _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
    }

    /// <summary>
    ///     Column name in the table.
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    ///     <para>
    ///         Specify it when its different from the property name.
    ///     </para>
    /// </remarks>
    public void ColumnName(string name)
    {
        _mapping.ColumnName = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    ///     The column is auto-incremented by the database engine.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Means that this data layer will not specify it but fetch it before/after the insert statement (depending on the
    ///         DB engine).
    ///     </para>
    /// </remarks>
    public void AutoIncrement()
    {
        _mapping.IsAutoIncrement = true;
    }
}