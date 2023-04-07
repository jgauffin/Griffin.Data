using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Griffin.Data.Configuration;

namespace Griffin.Data.Mappings;

/// <summary>
///     Implementation of <see cref="IMappingRegistry" />.
/// </summary>
/// <remarks>
///     <para>
///         Scans assemblies after classes that inherit <see cref="ClassMappingConfigurator{T}" /> and uses them to
///         configure mappings.
///     </para>
/// </remarks>
public class MappingRegistry : IMappingRegistry
{
    private readonly Dictionary<Type, ClassMapping> _mappings = new();
    private readonly List<Assembly> _scannedAssemblies = new();

    /// <inheritdoc />
    public ClassMapping Get<T>()
    {
        var type = typeof(T);
        if (!_mappings.Any())
        {
            Scan(Assembly.GetExecutingAssembly());
            Scan(type.Assembly);
        }

        var str = string.Join(", ", _scannedAssemblies.Select(x => x.GetName().Name));
        if (!_mappings.TryGetValue(type, out var mapper))
        {
            throw new MissingMappingException(type, str);
        }

        return mapper;
    }

    /// <inheritdoc />
    public ClassMapping Get([NotNull] Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!_mappings.Any())
        {
            Scan(Assembly.GetExecutingAssembly());
            Scan(type.Assembly);
        }

        var str = string.Join(", ", _scannedAssemblies.Select(x => x.GetName().Name));
        if (!_mappings.TryGetValue(type, out var mapper))
        {
            throw new MissingMappingException(type, str);
        }

        return mapper;
    }

    /// <summary>
    ///     Add a mapping.
    /// </summary>
    /// <param name="mapping">Mapping to add.</param>
    /// <remarks>
    ///     <para>
    ///         Will replace any existing mapping for the specified entity type.
    ///     </para>
    /// </remarks>
    public void Add(ClassMapping mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        _mappings[mapping.EntityType] = mapping;
    }

    /// <summary>
    ///     Find a specific mapping.
    /// </summary>
    /// <typeparam name="T">Type of entity to find a mapping for.</typeparam>
    /// <returns>Mapping if found; otherwise <c>null</c>.</returns>
    public ClassMapping? Find<T>()
    {
        var type = typeof(T);
        if (!_mappings.Any())
        {
            Scan(Assembly.GetExecutingAssembly());
            Scan(type.Assembly);
        }

        return _mappings.TryGetValue(type, out var mapper) ? mapper : null;
    }

    /// <summary>
    ///     Scan an assembly after implementations of <see cref="ClassMappingConfigurator{TEntity}" />.
    /// </summary>
    /// <param name="assembly">Assembly to scan.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Scan([NotNull] Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (_scannedAssemblies.Contains(assembly))
        {
            throw new InvalidOperationException($"Assembly '{assembly.GetName().Name}' has already been scanned.");
        }

        _scannedAssemblies.Add(assembly);

        var types = from type in assembly.GetTypes()
            let ifs = type.GetInterfaces()
            let mappingInterface = ifs.FirstOrDefault(x => x.Name.StartsWith("IEntityConfigurator"))
            where mappingInterface != null
            select new { type, mappingInterface };

        var builders = new List<IMappingBuilder>();
        foreach (var pair in types)
        {
            var entityType = pair.mappingInterface.GetGenericArguments()[0];
            var configType = typeof(ClassMappingConfigurator<>).MakeGenericType(entityType);
            var configurator = (IMappingBuilder)Activator.CreateInstance(configType)!;
            builders.Add(configurator);

            var mapping = Activator.CreateInstance(pair.type);
            var method = pair.type.GetMethod("Configure");
            if (method == null)
            {
                throw new InvalidOperationException($"Failed to find 'Configure' in {pair.type}.");
            }

            method.Invoke(mapping, new object[] { configurator });

            _mappings[entityType] = configurator.BuildMapping();
        }

        foreach (var mapping in builders)
        {
            mapping.BuildRelations(this);
        }
    }
}
