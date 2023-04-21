using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     Mapping between a parent and or or more children.
/// </summary>
/// <typeparam name="TParent">Parent entity type.</typeparam>
/// <typeparam name="TChild">Child entity type.</typeparam>
public class HasManyMapping<TParent, TChild> : RelationShipBase<TParent, TChild>, IHasManyMapping where TChild : notnull
{
    private readonly Func<TParent, object?> _getter;
    private readonly Action<TParent, object> _setter;

    /// <summary>
    /// </summary>
    /// <param name="fk">Foreign key mapping.</param>
    /// <param name="getter">Function to get the collection property value.</param>
    /// <param name="setter">Action to set the collection property value.</param>
    public HasManyMapping(
        ForeignKeyMapping<TParent, TChild> fk,
        Func<TParent, object?> getter,
        Action<TParent, object> setter) : base(fk, typeof(TChild))
    {
        _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        _setter = setter ?? throw new ArgumentNullException(nameof(setter));
    }

    /// <summary>
    ///     Column used to limit returned rows for the child table.
    /// </summary>
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
    public void SetCollection([NotNull] object instance, IList collectionInstance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        _setter((TParent)instance, collectionInstance);
    }

    /// <inheritdoc />
    public IList? GetCollection([NotNull] object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var value = (IList?)_getter((TParent)entity);
        if (value != null)
        {
            return value;
        }

        value = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(TChild)))!;
        _setter((TParent)entity, value);
        return value;
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
