using System;

namespace Griffin.Data.Mappings;

public interface IMappingRegistry
{
    ClassMapping Get<T>();
    ClassMapping Get(Type type);
}