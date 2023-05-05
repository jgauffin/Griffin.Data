using System;
using System.Reflection;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.Configuration;

/// <summary>
///     Fluent extensions for the configuration.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    ///     Add an assembly that contains mappings to the registry.
    /// </summary>
    /// <param name="configuration">config.</param>
    /// <param name="assembly">Assembly that contains mappings.</param>
    /// <returns>config.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DbConfiguration AddMappingAssembly(this DbConfiguration configuration, Assembly assembly)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (configuration.MappingRegistry is MappingRegistry reg)
        {
            reg.Scan(assembly);
        }

        return configuration;
    }

    /// <summary>
    ///     Add an assembly that contains mappings to the registry.
    /// </summary>
    /// <param name="configuration">config.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DbConfiguration AddMappingAssemblyByType<TMapping>(this DbConfiguration configuration)
    {
        return configuration.AddMappingAssembly(typeof(TMapping).Assembly);
    }

    /// <summary>
    ///     Enable snapshot change tracking.
    /// </summary>
    /// <param name="configuration">config.</param>
    /// <returns></returns>
    public static DbConfiguration UseSnapshotChangeTracking(this DbConfiguration configuration)
    {
        configuration.ChangeTrackerFactory = () => new SnapshotChangeTracking(configuration.MappingRegistry);
        return configuration;
    }
}
