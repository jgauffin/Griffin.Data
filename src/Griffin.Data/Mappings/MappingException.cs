using System;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// A mapping is incorrect
    /// </summary>
    public class MappingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingException" /> class.
        /// </summary>
        /// <param name="errMsg">Error message.</param>
        public MappingException(string errMsg)
            : base(errMsg)
        {
        }
    }
}