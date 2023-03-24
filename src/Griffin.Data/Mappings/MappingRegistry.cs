using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Griffin.Data.Configuration;

namespace Griffin.Data.Mappings;

public class MappingRegistry : IMappingRegistry
{
    private readonly Dictionary<Type, ClassMapping> _mappings = new();
    private readonly List<Assembly> _scannedAssemblies = new();

    public ClassMapping Get<T>()
    {
        var type = typeof(T);
        if (!_mappings.Any())
        {
            Scan(Assembly.GetExecutingAssembly());
            Scan(type.Assembly);
        }

        if (!_mappings.TryGetValue(type, out var mapper))
            throw new InvalidOperationException($"Failed to find a mapping for {type}.");

        return mapper;
    }

    public ClassMapping Get(Type type)
    {
        if (!_mappings.Any())
        {
            Scan(Assembly.GetExecutingAssembly());
            Scan(type.Assembly);
        }

        if (!_mappings.TryGetValue(type, out var mapper))
            throw new InvalidOperationException($"Failed to find a mapping for {type}.");

        return mapper;
    }

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

    public void Scan(Assembly assembly)
    {
        if (_scannedAssemblies.Contains(assembly))
            throw new InvalidOperationException($"Assembly '{assembly.GetName().Name}' has already been scanned.");

        _scannedAssemblies.Add(assembly);

        var types = from type in assembly.GetTypes()
            let ifs = type.GetInterfaces()
            let mappingInterface = ifs.FirstOrDefault(x => x.Name.StartsWith("IEntityConfigurator"))
            where mappingInterface != null
            select new { type, mappingInterface };

        foreach (var pair in types)
        {
            var entityType = pair.mappingInterface.GetGenericArguments()[0];
            var configType = typeof(ClassMappingConfigurator<>).MakeGenericType(entityType);
            var config = (IMappingBuilder)Activator.CreateInstance(configType)!;

            var configurator = Activator.CreateInstance(pair.type);
            var method = pair.type.GetMethod("Configure");
            if (method == null) throw new InvalidOperationException($"Failed to find 'Configure' in {pair.type}.");

            method.Invoke(configurator, new object[] { config });

            _mappings[entityType] = config.BuildMapping();
        }

        foreach (var mapping in _mappings.Values)
        {
            foreach (var collection in mapping.Collections)
            {
                collection.ForeignKey.ReferencedProperty =
                    mapping.GetProperty(collection.ForeignKey.ReferencedPropertyName!);
                var childMapping = _mappings[collection.ElementType];
                collection.ForeignKey.ForeignKey =
                    childMapping.GetProperty(collection.ForeignKey.ForeignKeyPropertyName);
            }

            foreach (var collection in mapping.Children)
            {
                collection.ForeignKey.ReferencedProperty =
                    mapping.GetProperty(collection.ForeignKey.ReferencedPropertyName!);
                var childMapping = _mappings[collection.ChildEntityType];
                collection.ForeignKey.ForeignKey =
                    childMapping.GetProperty(collection.ForeignKey.ForeignKeyPropertyName);
            }
        }
    }
}