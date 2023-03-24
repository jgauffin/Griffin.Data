using System;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Configuration;

public class KeyConfigurator<TEntity, TProperty>
{
    private readonly KeyMapping _mapping;

    public KeyConfigurator(KeyMapping mapping)
    {
        _mapping = mapping;
    }

    public void ColumnName(string name)
    {
        _mapping.ColumnName = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void AutoIncrement()
    {
        _mapping.IsAutoIncrement = true;
    }
}