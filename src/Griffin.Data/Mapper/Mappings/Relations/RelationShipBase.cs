using System;
using System.Collections;
using System.Collections.Generic;
using Griffin.Data.Mapper;

namespace Griffin.Data.Mapper.Mappings.Relations;

/// <summary>
/// </summary>
/// <typeparam name="TParent"></typeparam>
/// <typeparam name="TChild"></typeparam>
public abstract class RelationShipBase<TParent, TChild> : IRelationShip
{
    private readonly ForeignKeyMapping<TParent, TChild> _fk;

    /// <summary>
    /// </summary>
    /// <param name="fk"></param>
    /// <param name="childEntityType"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected RelationShipBase(ForeignKeyMapping<TParent, TChild> fk, Type? childEntityType)
    {
        _fk = fk ?? throw new ArgumentNullException(nameof(fk));
        ChildEntityType = childEntityType ?? typeof(TChild);
    }

    /// <summary>
    ///     Has a property with the foreign key.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ForeignKeyColumnName" /> must be specified if the child entity do not have a FK property.
    ///     </para>
    /// </remarks>
    public bool HasForeignKeyProperty => _fk.HasProperty;

    /// <summary>
    ///     Type of child entity.
    /// </summary>
    public Type ChildEntityType { get; }

    /// <inheritdoc />
    public string ForeignKeyColumnName => _fk.ForeignKeyColumnName;

    /// <inheritdoc />
    public object? GetReferencedId(object parent)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        return _fk.GetReferencedId((TParent)parent);
    }

    /// <inheritdoc />
    public object? GetForeignKeyValue(object childEntity)
    {
        if (childEntity == null)
        {
            throw new ArgumentNullException(nameof(childEntity));
        }

        if (_fk == null)
        {
            throw new MappingException(childEntity,
                "A foreign key property has not been specified. Either configure it or use the FK column name instead.");
        }

        return _fk.GetColumnValue(childEntity);
    }

    /// <inheritdoc />
    public void SetForeignKey(object childEntity, object fkValue)
    {
        if (childEntity == null)
        {
            throw new ArgumentNullException(nameof(childEntity));
        }

        if (_fk == null)
        {
            throw new MappingException(childEntity,
                "A foreign key property has not been specified. Either configure it or use the FK column name instead.");
        }

        _fk.SetColumnValue(childEntity, fkValue);
    }

    /// <inheritdoc />
    public virtual IDictionary<string, object> CreateDbConstraints(IEnumerable parentEntities)
    {
        var keys = new List<object>();
        foreach (var parent in parentEntities)
        {
            var id = GetReferencedId(parent);
            if (id == null)
            {
                continue;
            }

            keys.Add(id);
        }

        var parameters = new Dictionary<string, object>();

        ApplyConstraints(parameters);

        parameters.Add(_fk.ForeignKeyColumnName, keys.Count == 1 ? keys[0]! : keys);
        return parameters;
    }

    /// <summary>
    ///     Used in sub classes to add more db parameters.
    /// </summary>
    /// <param name="dbParameters">Collection to add items to. Keys should be column names.</param>
    protected virtual void ApplyConstraints(IDictionary<string, object> dbParameters)
    {
    }
}
