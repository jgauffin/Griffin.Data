using System;
using System.Data;

namespace Griffin.Data.Mappings.Properties;

public class PropertyMapping : IPropertyAccessor, IFieldMapping
{
    private bool _checkConverters = true;
    private readonly Func<object, object>? _getter;
    private Type _propertyType;
    private readonly Action<object, object>? _setter;

    public PropertyMapping(Func<object, object>? getter, Action<object, object>? setter)
    {
        _getter = getter;
        _setter = setter;
        CanWriteToDatabase = _getter != null;
        CanReadFromDatabase = _setter != null;
    }

    public Type PropertyType { get; set; }
    public bool CanReadFromDatabase { get; set; }
    public bool CanWriteToDatabase { get; set; }

    public Func<object, object>? PropertyToColumnConverter { get; set; }
    public Func<object, object>? ColumnToPropertyConverter { get; set; }
    public Func<IDataRecord, object>? RecordToColumnConverter { get; set; }

    public string ColumnName { get; set; }
    public string PropertyName { get; set; }

    public void SetColumnValue(object instance, object value)
    {
        if (_setter == null)
            throw new InvalidOperationException("No setter has been defined.");

        if (ColumnToPropertyConverter != null)
            value = ColumnToPropertyConverter(value);
        else if (_checkConverters) _checkConverters = false;
        //CreateConverters(value.GetType());
        _setter(instance, value);
    }

    public object? GetColumnValue(object entity)
    {
        if (_getter == null)
            throw new InvalidOperationException("No setter has been defined.");

        var value = _getter(entity);
        if (value != null && PropertyToColumnConverter != null) return PropertyToColumnConverter(value);

        return value;
    }
}