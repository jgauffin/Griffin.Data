using System;

namespace Griffin.Data.Configuration;

internal class MappingConfigurationException : Exception
{
    public MappingConfigurationException(Type entityType)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    public Type EntityType { get; }
}