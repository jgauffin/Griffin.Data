using System;
using System.Collections;
using System.Threading.Tasks;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

public class HasManyMapping : IPropertyAccessor
{
    private readonly Action<object, object> _addMethod;
    private readonly Func<IList> _collectionFactory;
    private readonly Func<object, object?> _getter;
    private readonly Action<object, object>? _setter;
    private readonly Func<object, Func<object, Task>, Task> _visitorWrapper;

    public HasManyMapping(string propertyName, Type propertyType, Type elementType, Func<object, object?> getter,
        Action<object, object>? setter, Func<IList> collectionFactory, Action<object, object> addMethod,
        Func<object, Func<object, Task>, Task> visitorWrapper)
    {
        _getter = getter;
        _setter = setter;
        _collectionFactory = collectionFactory;
        _addMethod = addMethod;
        _visitorWrapper = visitorWrapper;
        PropertyName = propertyName;
        PropertyType = propertyType;
        ElementType = elementType;
    }

    public string PropertyName { get; }
    public Type PropertyType { get; }
    public Type ElementType { get; }

    public ForeignKeyMapping ForeignKey { get; set; }


    public void SetColumnValue(object instance, object value)
    {
        if (_setter == null) throw new InvalidOperationException("No setter has been specified.");

        _setter(instance, value);
    }

    public object? GetColumnValue(object entity)
    {
        return _getter(entity);
    }

    public IList CreateCollection()
    {
        return _collectionFactory();
    }

    public void AddItem(object collection, object entity)
    {
        _addMethod(collection, entity);
    }

    public async Task Visit(object collection, Func<object, Task> itemCallback)
    {
        await _visitorWrapper(collection, itemCallback);
    }
}