using System;

namespace Griffin.Data.Converters
{
    /// <summary>
    /// Will change <c>DBNull.Value</c> to the specified default value (or <c>null</c> if none is specified)
    /// </summary>
    public class DbNullConverter : IColumnConverter
    {
        private object _defaultValue;

        public DbNullConverter()
        {
            _defaultValue = null;
        }

        public DbNullConverter(object defaultValue)
        {
            if (defaultValue == null) throw new ArgumentNullException("defaultValue");
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Convert from db value to property value
        /// </summary>
        /// <param name="dbColumnValue">Value in the db column</param>
        /// <returns>Value which can be assigned to the property</returns>
        public object Convert(object dbColumnValue)
        {
            return dbColumnValue == DBNull.Value ? _defaultValue : dbColumnValue;
        }
    }
}
