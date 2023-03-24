using System;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

public class HasOneMapping : IPropertyAccessor
{
    private readonly Func<object, object?>? _getter;
    private readonly Action<object, object>? _setter;

    /// <summary>
    /// </summary>
    /// <param name="parentPropertyName">Property that the child entity is assigned to.</param>
    public HasOneMapping(string parentPropertyName, Func<object, object?>? getter, Action<object, object>? setter)
    {
        ParentPropertyName = parentPropertyName;
        _getter = getter;
        _setter = setter;
    }

    public string ParentPropertyName { get; }

    public ForeignKeyMapping ForeignKey { get; set; }
    public Type ParentEntityType { get; set; }
    public Type ChildEntityType { get; set; }

    public void SetColumnValue(object instance, object value)
    {
        if (_setter == null) throw new InvalidOperationException("No setter has been specified");

        _setter(instance, value);
    }

    public object? GetColumnValue(object entity)
    {
        if (_getter == null) throw new InvalidOperationException("No getter has been specified");

        return _getter(entity);
    }
}