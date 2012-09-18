using System;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Uses <see cref="Convert.ChangeType(object, Type)"/> for the conversion.
    /// </summary>
    /// <typeparam name="TEntityValue">Property type</typeparam>
    /// <remarks>Uses <c>Activator.CreateInstance</c> to create default values if the db value is null. Not very effecient, but the
    /// best I could come up with. (primitives is the problem)</remarks>
    public class DotNetConverter<TEntityValue> : IColumnConverter
    {

        /// <summary>
        /// Convert from db value to property value
        /// </summary>
        /// <param name="dbColumnValue">Value in the db column</param>
        /// <returns>Value which can be assigned to the property</returns>
        public object ConvertFromDb(object dbColumnValue)
        {
            var isString = typeof (TEntityValue) == typeof (string);
            if (dbColumnValue == null || dbColumnValue == DBNull.Value)
                return isString ? null : Activator.CreateInstance(typeof(TEntityValue)); // to get default value

            return isString
                       ? dbColumnValue.ToString()
                       : Convert.ChangeType(dbColumnValue, typeof (TEntityValue));
        }
    }

}