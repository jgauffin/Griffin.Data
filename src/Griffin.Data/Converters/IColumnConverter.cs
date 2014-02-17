namespace Griffin.Data.Converters
{
    /// <summary>
    /// Used to convert from a table column type to a property type
    /// </summary>
    public interface IColumnConverter
    {
        /// <summary>
        /// Convert from db value to property value
        /// </summary>
        /// <param name="dbColumnValue">Value in the db column</param>
        /// <returns>Value which can be assigned to the property</returns>
        object Convert(object dbColumnValue);
    }
}