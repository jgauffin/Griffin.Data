﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Griffin.Data.Converters.Enums;

/// <summary>
///     Convert a generic (i.e. a enum type) to another data type.
/// </summary>
/// <typeparam name="TColumn"></typeparam>
/// <typeparam name="TEnum"></typeparam>
/// <remarks>
///     <para>
///         There is no struct restriction since this convert can not be used by the property mappings with it.
///     </para>
/// </remarks>
public class GenericToEnumConverter<TColumn, TEnum> : ISingleValueConverter<TColumn, TEnum>
{
    private readonly Func<TColumn, TEnum> _converter;
    private readonly bool _isFlags;
    private readonly TColumn[] _values;

    /// <summary>
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public GenericToEnumConverter()
    {
        if (!typeof(TEnum).IsEnum) throw new ArgumentException("Type must be an enum", nameof(TEnum));

        var values = Enum.GetValues(typeof(TEnum));
        _isFlags = typeof(TEnum).GetCustomAttribute<FlagsAttribute>() != null;

        if (!_isFlags)
        {
            var sourceType = typeof(TColumn);
            _values = new TColumn[values.Length];
            for (var i = 0; i < _values.Length; i++)
            {
                var value = values.GetValue(i);
                _values[i] = (TColumn)Convert.ChangeType(value, sourceType);
            }
        }
        else
        {
            _values = Array.Empty<TColumn>();
        }

        var p = Expression.Parameter(typeof(TColumn));
        var c = Expression.ConvertChecked(p, typeof(TEnum));
        _converter = Expression.Lambda<Func<TColumn, TEnum>>(c, p).Compile();
    }

    public TEnum ColumnToProperty(TColumn value)
    {
        if (!_isFlags) EnsureEnumValue(value);

        return _converter(value);
    }

    public TColumn PropertyToColumn(TEnum value)
    {
        return (TColumn)Convert.ChangeType(value, typeof(TColumn));
    }

    protected void EnsureEnumValue(TColumn value)
    {
        var isFound = false;
        foreach (var enumValue in _values)
            if (enumValue?.Equals(value) == true)
                isFound = true;

        if (!isFound) throw new InvalidOperationException($"Failed to find value {value} in enum {typeof(TEnum)}.");
    }
}