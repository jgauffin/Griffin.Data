using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mappings;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Makes the life a bit easier by supporting the most simple queries.
    /// </summary>
    /// <remarks>
    /// <para>To enable these features you must use mappings with implements <see cref="ITableMapping"/>, for instance by using <see cref="EntityMapper{T}"/> instead of <see cref="SimpleMapper{TEntity}"/>.</para>
    /// <para>
    /// The library also provides a way for you to handle transactions through your IoC container. Example registration:
    /// <code>
    /// <![CDATA[
    /// containerRegistrar.RegisterInstance<IConnectionFactory>(new AppConfigConnectionFactory("theConString"));
    /// containerRegistrar.RegisterType<IAdoNetContext, AdoNetContext>(Lifetime.Scoped);
    /// ]]>
    /// </code>
    /// </para>
    /// </remarks>
    /// <example>
    /// </example>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
