using System;
using System.Collections;
using System.Collections.Generic;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     Base interface or OneToMany and OneToOne.
/// </summary>
public interface IRelationShip
{
    /// <summary>
    ///     Type of child (or element type for collections).
    /// </summary>
    Type ChildEntityType { get; }

    /// <summary>
    ///     Column name in the child table for the foreign key.
    /// </summary>
    string ForeignKeyColumnName { get; }

    /// <summary>
    ///     Has a foreign key property configured.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If not, the <see cref="ForeignKeyColumnName" /> should be used when fetching children.
    ///     </para>
    /// </remarks>
    bool HasForeignKeyProperty { get; }

    /// <summary>
    ///     Create constrains as DB parameters (i.e. using column names).
    /// </summary>
    /// <param name="parentEntities"></param>
    /// <returns>
    ///     Key is the FK column name and the values is either a single value or an array of values (depending on if one
    ///     or multiple parent entities was specified).
    /// </returns>
    IDictionary<string, object> CreateDbConstraints(IEnumerable parentEntities);

    /// <summary>
    ///     Get value from the foreign key property in the child table.
    /// </summary>
    /// <param name="childEntity">instance to get key from.</param>
    /// <returns>Key if specified; otherwise <c>null</c>.</returns>
    object? GetForeignKeyValue(object childEntity);

    /// <summary>
    ///     Get key in the parent entity that the foreign key points at.
    /// </summary>
    /// <param name="parent">Parent entity.</param>
    /// <returns>Value if specified; otherwise <c>null</c>.</returns>
    object? GetReferencedId(object parent);

    /// <summary>
    ///     Set foreign key value in the child entity.
    /// </summary>
    /// <param name="childEntity">Instance to assign property value to..</param>
    /// <param name="fkValue">Value to assign.</param>
    void SetForeignKey(object childEntity, object fkValue);
}
