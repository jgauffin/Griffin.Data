using System;
using System.Linq;
using Griffin.Data.Mappings;

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
        var values = mapping.Keys.Select(x => x.GetColumnValue(entity)).Where(x => x != null).ToList();
        if (!values.Any())
        {
            return null;
        }

        var key = entity.GetType().Name;
        return $"{key}{string.Join(", ", values)}";
    }
}
