using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Mapping for a specific column
    /// </summary>
    /// <typeparam name="TEntity">POCO</typeparam>
    public class SimpleColumnMapping<TEntity> : IColumnMapping
    {
        private readonly string _columnName;
        private readonly IColumnConverter _converter;
        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleColumnMapping{TEntity}" /> class.
        /// </summary>
        /// <param name="property">The property that the mapping is for.</param>
        /// <param name="columnName">Name of the column in the table.</param>
        /// <param name="converter">Used of the column value is not of the same type as the property.</param>
        public SimpleColumnMapping(Expression<Func<TEntity, object>> property, string columnName,
                                   IColumnConverter converter)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (columnName == null) throw new ArgumentNullException("columnName");

            _columnName = columnName;
            _converter = converter;
            var member = property.GetMemberInfo();
            _propertyInfo = (PropertyInfo)member.Member;
        }

        /// <summary>
        /// Assign the property
        /// </summary>
        /// <param name="record">A data record</param>
        /// <param name="entity">The entity that the property exists in</param>
        public void SetValue(IDataRecord record, object entity)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (entity == null) throw new ArgumentNullException("entity");

            var value = record[_columnName];
            if (_converter != null)
                value = _converter.ConvertFromDb(value);

            _propertyInfo.SetValue(entity, value, null);
        }
    }
}