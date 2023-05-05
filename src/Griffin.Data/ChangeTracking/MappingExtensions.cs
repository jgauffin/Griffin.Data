using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.ChangeTracking;

/// <summary>
///     Mapping extensions for change tracking.
/// </summary>
internal static class MappingExtensions
{
    public static string? GenerateKey(this IMappingRegistry registry, object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return registry.Get(entity.GetType()).GenerateKey(entity);
    }

    public static string? GenerateKey(this ClassMapping mapping, object entity)
    {
        var keys = new List<object>();
        foreach (var keyMapping in mapping.Keys)
        {
            var value = keyMapping.GetColumnValue(entity);

            // any of the keys being null = new item.
            if (value == null)
            {
                return null;
            }

            keys.Add(value);
        }

        if (!keys.Any())
        {
            return null;
        }

        var key = entity.GetType().Name;
        return $"{key}{string.Join(", ", keys)}";
    }
}
