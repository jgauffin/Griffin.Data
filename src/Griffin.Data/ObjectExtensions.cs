using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data
{
    /// <summary>
    /// Extensions for <c>object</c>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks if the specified object is null or <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="instance">obj</param>
        /// <returns>true if so; otherwise false.</returns>
        public static bool IsNull(this object instance)
        {
            return instance == null || instance == DBNull.Value;
        }
    }
}
