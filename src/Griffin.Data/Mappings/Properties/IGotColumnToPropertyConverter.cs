using System;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Defines that a property or a key has a converter.
/// </summary>
/// <typeparam name="TProperty">Type of property.</typeparam>
public interface IGotColumnToPropertyConverter<in TProperty>
{
    /// <summary>
    ///     Delegate to convert property value to a column value.
    /// </summary>
    Func<TProperty, object>? PropertyToColumnConverter { get; }
}
