using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Mapper;

namespace Griffin.Data.Mappings.Relations;

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
        var parents = (IReadOnlyList<TParent>)parentEntities;

        var parameters = new Dictionary<string, object>();

        ApplyConstraints(parameters);

        var ids = parents.Select(x => GetReferencedId(x!)).Where(x => x != null).ToList();
        parameters.Add(_fk.ForeignKeyColumnName, ids.Count == 1 ? ids[0]! : ids);
        return parameters;
    }

    protected virtual void ApplyConstraints(IDictionary<string, object> dbParameters)
    {
    }
}
