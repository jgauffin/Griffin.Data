using System;
using System.Linq.Expressions;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
/// Foreign key configuration for a has one/many mapping.
/// </summary>
/// <typeparam name="TParentEntity">Entity that the FK points at.</typeparam>
public class ForeignKeyConfiguration<TParentEntity>
{
    private readonly ForeignKeyMapping _mapping;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapping">Mapping object to add information to.</param>
    public ForeignKeyConfiguration(ForeignKeyMapping mapping)
    {
        _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
    }

    /// <summary>
    /// Property in the parent entity that the FK points at.
    /// </summary>
    /// <typeparam name="TReferencedProperty">Referenced property type.</typeparam>
    /// <param name="referencedPropertySelector">Expression used to select the property.</param>
    public void References<TReferencedProperty>(
        Expression<Func<TParentEntity, TReferencedProperty>> referencedPropertySelector)
    {
        _mapping.ReferencedPropertyName = referencedPropertySelector.GetMemberName();
    }

    //public void References(string columnName)
    //{
    //    _mapping.ReferencedColumnName = columnName;

    //}
    //public void References(string tableName, string columnName)
    //{
    //    _mapping.ReferencedTableName = tableName;
    //    _mapping.ReferencedColumnName = columnName;
    //}
}