using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
/// Mapping between a parent and or or more children.
/// </summary>
/// <typeparam name="TParent">Parent entity type.</typeparam>
/// <typeparam name="TChild">Child entity type.</typeparam>
public class HasManyMapping<TParent, TChild> : RelationShipBase<TParent, TChild>, IHasManyMapping where TChild: notnull
{
    private readonly Func<TParent, object> _getter;
    private readonly Action<TParent, object> _setter;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fk">Foreign key mapping.</param>
    /// <param name="getter">Function to get the collection property value.</param>
    /// <param name="setter">Action to set the collection property value.</param>
    public HasManyMapping(ForeignKeyMapping<TParent, TChild> fk, Func<TParent, object> getter,
        Action<TParent, object> setter) : base(fk, typeof(TChild))
    {
        _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        _setter = setter ?? throw new ArgumentNullException(nameof(setter));
    }

    public KeyValuePair<string, string>? SubsetColumn { get; set; }

    /// <inheritdoc />
    public IList CreateCollection()
    {
        return new List<TChild>();
    }

    /// <inheritdoc />
    public async Task Visit(object collection, Func<object, Task> visitor)
    {
        var col = (IReadOnlyList<TChild>)collection;
        foreach (var item in col)
        {
            await visitor(item);
        }
    }

    /// <inheritdoc />
    public void SetColumnValue([NotNull]object instance, object collectionInstance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        _setter((TParent)instance, collectionInstance);
    }

    /// <inheritdoc />
    public object? GetColumnValue([NotNull] object entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return _getter((TParent)entity);
    }

    /// <inheritdoc />
    protected override void ApplyConstraints(IDictionary<string, object> dbParameters)
    {
        if (SubsetColumn != null)
        {
            dbParameters.Add(SubsetColumn.Value.Key, SubsetColumn.Value.Value);
        }
    }
}
