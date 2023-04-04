using Griffin.Data.Configuration;
using Griffin.Data.Converters.Dates;
using Griffin.Data.Converters.Enums;
using System;

namespace Griffin.Data.Converters;

/// <summary>
///     Extension methods for enums.
/// </summary>
public static class ConverterExtensions
{
    /// <summary>
    /// Convert a varchar() column to an enum property.
    /// </summary>
    /// <typeparam name="TEntity">Entity that contains the enum property.</typeparam>
    /// <typeparam name="TProperty">Type of enum.</typeparam>
    /// <returns>Property configurator.</returns>
    public static PropertyConfigurator<TEntity, TProperty> StringEnum<TEntity, TProperty>(
        this PropertyConfigurator<TEntity, TProperty> prop) where TProperty : struct
    {
        prop.Converter(new StringToEnum<TProperty>());
        return prop;
    }

    /// <summary>
    /// Convert an int column to an enum property.
    /// </summary>
    /// <typeparam name="TEntity">Entity that contains the enum property.</typeparam>
    /// <typeparam name="TProperty">Type of enum.</typeparam>
    /// <returns>Property configurator.</returns>
    public static PropertyConfigurator<TEntity, TProperty> IntEnum<TEntity, TProperty>(
        this PropertyConfigurator<TEntity, TProperty> prop) where TProperty : struct
    {
        prop.Converter(new IntToEnum<TProperty>());
        return prop;
    }

    /// <summary>
    /// Convert a UTC date column to a local DateTime property.
    /// </summary>
    /// <typeparam name="TEntity">Entity that contains the enum property.</typeparam>
    /// <returns>Property configurator.</returns>
    public static PropertyConfigurator<TEntity, DateTime> UtcToLocal<TEntity>(
        this PropertyConfigurator<TEntity, DateTime> prop)
    {
        prop.Converter(new UtcToLocal());
        return prop;
    }

    /// <summary>
    /// Convert a unix epoch (ms) column to a DateTime property.
    /// </summary>
    /// <typeparam name="TEntity">Entity that contains the enum property.</typeparam>
    /// <returns>Property configurator.</returns>
    public static PropertyConfigurator<TEntity, DateTime> UnixEpoch<TEntity>(
        this PropertyConfigurator<TEntity, DateTime> prop)
    {
        prop.Converter(new UnixEpochConverter());
        return prop;
    }

}