using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// This is a simple mapping layer for ADO.NET.
    /// </summary>
    /// <remarks>
    /// <para>Do note that it's a mapping layer and not a Object/Relation mapper. So you still have to write SQL queries, but doesn't have 
    /// to map the result by yourself. The layer do however aid you in creating simpler SQL statements</para>
    /// <para>
    /// Mappings comes in different flavors. You can create a custom mapping by using the <see cref="IDataRecordMapper{T}"/> directly,
    /// use lambda expressions to create a mapping: <see cref="SimpleMapper{T}"/> or point at a custom function for the mapping: <see cref="DelegateMapper{T}"/>.
    /// </para>
    /// <para>Next you'll have to specify where all mappings are located. You do that by using the <see cref="MapperProvider"/>. You can subclass it to provide mappings from an Inversion Of Control Container or similar.</para>
    /// <para>And finally use one of the extension methods on your <c>DbCommand</c>: <see cref="CommandExtensions"/></para>
    /// </remarks>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
