using System;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Use a lambda/delegate for the conversion
    /// </summary>
    /// <typeparam name="TFrom">DB column type</typeparam>
    /// <typeparam name="TTo">Property type</typeparam>
    public class DelegateConverter<TFrom, TTo> : IColumnConverter
    {
        private readonly Func<TFrom, TTo> _func;

        public DelegateConverter(Func<TFrom, TTo> func)
        {
            if (func == null) throw new ArgumentNullException("func");

            _func = func;
        }

        /// <summary>
        /// Convert from db value to property value
        /// </summary>
        /// <param name="dbColumnValue">Value in the db column</param>
        /// <returns>Value which can be assigned to the property</returns>
        public object ConvertFromDb(object dbColumnValue)
        {
            return _func((TFrom)dbColumnValue);
        }
    }
}