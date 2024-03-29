Configuration
=============

This library uses a class called `Session` to perform all operations. The session contains information such as configured mappings, a database transaction, an (optional) change-tracking implementation, and a SQL dialect implementation.
Because of that, you need to start by configuring which features to use.

Use the `DbConfiguration` class to configure this library.

At a minimum, configure the connection string and where to find mappings.

```csharp
var config =  new DbConfiguration();

// Scan the specified assembly after mappings.
config.RegisterMappings(typeof(OneMapping).Assembly);

// configuration = appSettings.json
config.ConnectionString = configuration.GetConnectionString("Db");
```

For DI users, add both classes to the container information. Here is a Microsoft DI container example:

```csharp
services.AddSingleton<DbConfiguration>();
services.AddScoped<Session>();
```

When not using a DI, create the `Session` as this:

```csharp
using (var session = new Session(config))
{
    var entity = session.GetById<User>(1);
    entity.FirstName = "Jonas";
    session.Update(entity);

    session.SaveChanges();
}
```

If you have enabled change tracking, you can skip the persistence step (i.e. calling Update).

```csharp
config.EnableSnapshotTracking();

using (var session = new Session(config))
{
    var entity = session.GetById<User>(1);
    entity.FirstName = "Jonas";
    session.SaveChanges();
}
```

## Configuring entities

Let's say that you have the following class and table:

```csharp
class User
{
    public int Id { get; }
    public string FirstName { get; set; }
}
```

```sql
create table Users
(
    Id int not null identity primary key,
    FirstName varchar(40) not null
)
```

To be able to connect them, you need to specify a binding.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        // Required since the table name differs from the class name
        config.TableName("Users");

        // required since we use an auto incremented column as PK.
        config.Key(x => x.Id).AutoIncrement();

        // Optional, but added to show how. Read more later.
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

To avoid a lot of simple configurations you can simply call `MapRemaningProperties()`:

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

`MapRemaningProperties()` works when the following rules are followed:

* The column and the property has the same name.
* The column and the property has the same type.
* The column is not auto-generated or requires a sequence/generator
* The property is not for a child entity.

```csharp
class User
{
    private List<Address> _addresses = new List<Address>();

    public User(string firstName){
        FirstName = firstName;
    }

    // Required by this library when non-default constructors exist.
    // Can be private to protect the state of the business entity.
    protected User() {

    }

    // Can be get-only.
    public int Id { get; }

    public string FirstName { get; set; }

    // Get only properties with a backing field are allowed
    // as long as the backing field is named the same.
    public IReadOnlyList<Address> Addresses => _addresses;

    public void Add(Address address)
    {
        // Checks etc to ensure the address is valid for this user.
        // [...]

        _addresses.Add(address);
    }
}

class Address
{
    // It can be get-only since this library will assign the FK
    // value before inserting.
    public int UserId { get; }

    public int PostalCode { get; set; }

    public string Country { get; set; }

    // Remaining fields.
}
```

## Conversions

Sometimes the type of the DB column and the class property does not match. As such, a conversion is required between the property and the column.

This library supports conversions for regular properties, and some conversions are built in.

## More information

* [Conversions](Conversions.md)
* [Mappings](Mappings.md)
* [Has many relationships](HasMany.md)
* [Has one relationships](HasOne.md)


