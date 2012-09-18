using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data
{
    /// <summary>
    /// A short abstract of an item
    /// </summary>
    public class ItemAbstract
    {
        /// <summary>
        /// Gets or sets item id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets title / display name / etc etc
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets short item description (used as a tooltip or whatever you like)
        /// </summary>
        public string Description { get; set; }
    }
}
