Mappings
========

Mappings are used to describe how a table can be mapped to a class.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        // Required since the table name differs from the class name
        config.TableName("Users");

        // required since we use a auto incremented column as PK.
        config.Key(x => x.Id).AutoIncrement();

        // Optional, but added to show how. Read more later.
        config.Property(x => x.FirstName);
    }
}
```

Mappings are automatically picked up by the `MappingRegistry` as long as you point to an assembly.

```csharp
var registry = new MappingRegistry();
registry.Scan(typeof(UserMapping).Assembly);
```

The mapping registry is in charge of loading and caching mappings. By default, the library uses the one declared as a property in `DbConfiguration`. 

# Table names vs class names

Table names are optional in the mappings as long as the table name is a plural version of the class name.
If they aren't, you need to turn that off:

```csharp
mappingRegistry.PluralizeTableNames = false;
```

If table names do not adhere to that rule, you can specify the table name in each mapping:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.TableName("ProdUsers");

        // [.. rest of the mapping...]
    }
}
```

# Column names vs property names

This library assumes that a property has the same name as the column. If that's not the case, you must explicitly tell the column name.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Property(x => x.FirstName).Column("first_name");

        // [.. rest of the mapping...]
    }
}
```

# Keys

The library uses keys to identify a specific row. Therefore, all tables **must** have primary keys. 

The change tracker also uses primary keys to identify two copies of the same entity.

Define keys like this:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.MyKey);

        // [.. rest of the mapping...]
    }
}
```

Add `.AutoIncrement()`  for keys that get auto-generated values from the database. It tells the library not to include them in the INSERT statement but fetches the value after the INSERT statement completes.

# Properties

Properties are fields that are not keys or child entities.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Property(x => x.FirstName);

        // [.. rest of the mapping...]
    }
}
```

## Conventional property mapping

No configuration is required for properties where the property and column name matches. Instead, invoke `MapRemainingProperties()` in the bottom of your mapping.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.MyKey).AutoIncrement();

        // [.. rest of the mapping...]

        config.MapRemainingProperties();
    }
}
```


# Relationships

* [Has many](HasMany.md)
* [Has one](HasOne.md)
