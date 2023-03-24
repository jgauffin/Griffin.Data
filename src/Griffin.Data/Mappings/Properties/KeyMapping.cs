using System;

namespace Griffin.Data.Mappings.Properties;

public class KeyMapping : IPropertyAccessor, IFieldMapping
{
    private readonly Func<object, object?>? _getter;
    private readonly Action<object, object>? _setter;

    public KeyMapping(Func<object, object?>? getter, Action<object, object>? setter)
    {
        _getter = getter;
        _setter = setter;
    }

    public bool IsAutoIncrement { get; internal set; }
    public string ColumnName { get; internal set; }
    public string PropertyName { get; internal set; }

    public void SetColumnValue(object instance, object value)
    {
        if (_setter == null)
            throw new InvalidOperationException("No setter has been defined.");

        _setter(instance, value);
    }

    public object? GetColumnValue(object entity)
    {
        if (_getter == null) throw new InvalidOperationException("No getter has been defined.");

        return _getter(entity);
    }
}