One to many
===========

One to many means that one class has a collection of children in a property.

```csharp
public class User
{
    public IList<Address> { get; set; }
}
```

If you are designing business entites, the above code is flawed, since the ´User` looses control of it's children. It cannot check if the child is valid for it, since children can be added directly to the collection.

A better design is to use `IReadOnlyList<Address>` interface since it prevents direct adds to the collection:


```csharp
public class User
{
    private List<Address> _addresses = new List<Address>();

    public IReadOnlyList<Address> Addresses => _addresses;

    public void Add(Address address)
    {
        if (address.Country != "Sweden") {
            throw new ArgumentException("Sorry, but this awesomeness is only for Swedes and other vegetables.");
        }

        _addresses.Add(address);
    }
}
```

A sharp eye mights see that the `Address` property do not have any accessors (get/set). That's OK as long as the backing field is named as the property (`SomeProperty => _someProperty`).

## Complete example

Here is a minimal complete example.

First we have a parent and child class:

```csharp
public class SomeParent
{
    public int Id { get; set; }
    public IList<SomeChild> Children { get; set; }
}

public class SomeChild
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string SomeProperty { get; set; }
}
```

The relationship configuration is done in the parent mapping.

```csharp
internal class SomeParentMapping : IEntityConfigurator<SomeParent>
{
    public void Configure(IClassMappingConfigurator<SomeParent> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.HasMany(x => x.Children)   // Child property in the arent class.
            .ForeignKey(x => x.ParentId)  // FK property in the child class (SomeChild).
            .References(x => x.Id);       // property in the main entity (SomeParent) that the FK references.
        
        config.MapRemaningProperties();
    }
}
```

The child class contains a standard mapping.

```csharp
internal class SomeChildMapping : IEntityConfigurator<SomeChild>
{
    public void Configure(IClassMappingConfigurator<SomeChild> config)
    {
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemaningProperties();
    }
}
```

Finally, two tables need to be defined.

```sql
create table SomeParents
(
    Id int not null identity primary key
)

create table SomeChildren
(
    Id int not null identity primary key,
    SomeParentId int not null constraint FK_SomeParentId_SomeParents foreign key references SomeParents(Id)
    SomeProperty varchar(40) not null
)
```

## Inheritance

Inheritance is not supported in has many relationships directly. You can however create a connection table that are used to tell which child type to load.

```csharp
public class SomeParent
{
    public int Id { get; set; }
    public IList<SomeChild> Children { get; set; }
}

public class ChildAction

public interface IAction
{

}

public class SomeChild
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string SomeProperty { get; set; }
}
```



## Restrictions

The following restrictions apply to has many mappings.

### Must be an interface

The collection property must be an interface and not a concrete type.

The following interfaces are supported:

* IEnumerable<T>
* IReadOnlyList<T>
* IList<T>
* T[] (arrays)

### Foreign key property

One to many relationships requires that the child table has a foreign key that points at a column in the parent entity.

Example:





#### Corresponding tables

```sql
create table Users
(
    Id int not null identity primary key,
    FirstName varchar(20) not null
)

create table Addresses
(
    Id int not null identity primary key,
    UserId int not null constraint FK_Addresses_Users foreign key references Users(Id)
    PostalCode int not null
)
```

#### Mappings

And finally the mappings.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.TableName("Users");
        config.Key(x => x.Id).AutoIncrement();

        config.HasMany(x => x.Addresses) // Child property
            .ForeignKey(x => x.UserId) // FK property in the child (address)
            .References(x => x.Id); // property in the main entity (User) that the FK references
        
        config.MapRemaningProperties();
    }
}

internal class AddressMapping : IEntityConfigurator<Address>
{
    public void Configure(IClassMappingConfigurator<Address> config)
    {
        config.TableName("Addresses");
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemaningProperties();
    }
}
```

Has-many properties must use `IList<>`, `ICollection<>` or `IReadOnlyList<>` as their type.