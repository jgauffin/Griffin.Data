using System;
using System.Linq;
using System.Reflection;

namespace Griffin.Data.Queries.Implementation;

internal class QueryHandlerFinder
{
    public void FindHandlers(Assembly assembly, Action<(Type handlerType, Type queryType, Type interfaceType)> visitor)
    {
        var ourTypes = from type in assembly.GetTypes()
                       where !type.IsAbstract && !type.IsInterface
                       from implementedInterface in type.GetInterfaces()
                       where implementedInterface.IsGenericType &&
                             typeof(IQueryHandler<,>) == implementedInterface.GetGenericTypeDefinition()
                       select new { Type = type, Interface = implementedInterface };
        foreach (var type in ourTypes)
        {
            var queryType = type.Interface.GetGenericArguments()[0];
            visitor((type.Type, queryType, type.Interface));
        }
    }
}
