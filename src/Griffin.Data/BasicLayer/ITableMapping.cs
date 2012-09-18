using Griffin.Data.Mappings;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// The mappings provided by <see cref="MapperProvider"/> must implement this interface to activate the convinience methods in this namespace.
    /// </summary>
    /// <remarks></remarks>
    public interface ITableMapping
    {
        /// <summary>
        /// Gets name of the table/view.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Gets id column name.
        /// </summary>
        string IdColumnName { get; }
    }
}