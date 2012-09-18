namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Used to convert the conversion to typed.
    /// </summary>
    /// <typeparam name="TColumnValue">Type of value in the db column.</typeparam>
    /// <typeparam name="TEntityValue">Property type</typeparam>
    public abstract class ColumnConverterBase<TColumnValue, TEntityValue> : IColumnConverter
    {
        object IColumnConverter.ConvertFromDb(object dbColumnValue)
        {
            return ConvertDbValue((TColumnValue)dbColumnValue);
        }

        /// <summary>
        /// Convert db value
        /// </summary>
        /// <param name="dbValue">Value in the db</param>
        /// <returns>Value which can be assigned to the property.</returns>
        public abstract TEntityValue ConvertDbValue(TColumnValue dbValue);
    }
}