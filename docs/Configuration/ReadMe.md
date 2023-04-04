Configuration
=============

This library uses a class called `DbSession` to perform all operations. The session contains information such as configured mappings, database transaction, change tracking and a SQL dialect implementation.
Because of that, you need to start by configuration which features to use.

That is done with the help of the `DbConfiguration` class.

At minimum configure the connection string and where to find mappings:

```csharp
var config =  new DbConfiguration();

// Scan the specified assembly after mappings.
config.RegisterMappings(typeof(OneMapping).Assembly);

// configuration = appSettings.json
config.ConnectionString = configuration.GetConnectionString("Db");
```

Once done, the configuration should be past to the `DbSession` class. If you use microsofts DI container you can configure it as follows:

```csharp
services.AddSingleton<DbConfiguration>();
services.AddScoped<DbSession>();
```

Or if don't use a container, you can just create it directly:

```csharp
using (var session = new DbSession(config))
{
    var entity = session.GetById<User>(1);
    entity.FirstName = "Jonas";
    session.Update(entity);

    session.SaveChanges();
}
```

If you have enabled change tracking, you can skip the persistance step (i.e. calling Update).

```csharp

config.EnableSnapshotTracking();

using (var session = new DbSession(config))
{
    var entity = session.GetById<User>(1);
    entity.FirstName = "Jonas";
    session.SaveChanges();
}
```

## Configuring entities

Let's say that you have the following class:

```csharp
class User
{
    public int Id { get; }
    public string FirstName { get; set; }
}
```

To use that, you need to create a mapping:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.FirstName);
    }
}
```

The mapping will automatically be picked up by the `DbConfiguration` class (as long as you tell it to scan the assembly that the mapping is in).

Creating mappings can be tedious if you have many fields in each class:

```csharp
class User
{
    public int Id { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Country { get; set; }
    public int ZipCode { get; set; }
    public AccountState State { get; set; }
}
```

To avoid a lot if simple configurations you can simply call `MapRemaningProperties()`:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemaningProperties();
    }
}
```

For that to work, some rules must be followed:

* The column and the property has the same name.
* The column and the property has the same type.
* The column is not auto-generated or requires a sequence/generator
* The property is not for a child entity.

### One to many

One to many relationships requires that the child table has a foreign key that points at a column in the parent entity.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.TableName("MainTable");
        config.Key(x => x.Id).AutoIncrement();

        config.HasMany(x => x.Addresses) // Child property
            .ForeignKey(x => x.UserId) // FK property in the child (address)
            .References(x => x.Id); // property in the main entity (User) that the FK references
        
        config.MapRemaningProperties();
    }
}
```

Has-many properties must use `IList<>`, `ICollection<>` or `IReadOnlyList<>`.

### One to one (1 .. 0-1)

One to zero/one relationships work 