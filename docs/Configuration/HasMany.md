One to many
===========

One-to-many means that one class has a collection of children on a property.

```csharp
public class User
{
    public IList<Address> Addresses { get; set; }
}
```

If you design business entities, the above code is flawed since the ´User` loses control of its children. Furthermore, it cannot check if the child is valid for it since children can be added directly to the collection.

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

A sharp eye might see that the `Address` property has no accessors (get/set). That's OK if the backing field is named as the property (`SomeProperty => _someProperty`).

## Complete example

Here is a minimal complete example.

First, we have a parent and child class:

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

        config.HasMany(x => x.Children)   // Child property in the parent class.
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

Inheritance in one-to-many relationships is challenging since the collection of children can have different subclasses. That means that the parent table cannot contain the type, nor the child tables (as that would require a query against all possible child tables).

Therefore, inheritance is not supported in has-many relationships directly. You can, however, create a connection table that is used to tell which child type to load.

```csharp
public class SomeParent
{
    public int Id { get; set; }
    public IList<ChildConnector> Children { get; set; }
}

// Class used to tell which type the child is
public class ChildConnector
{
    public int Id { get; set; }

    // Must be included so that the correct connectors
    // are loaded by the library.
    public int ParentId  { get; set; }

    // This property is used to load the correct type
    // The type of field doesn't matter for the library.
    // enum, string, or int are the most common.
    public string DataType { get; set; }

    // The correct child is loaded into this property.
    public IData Data { get; set; }
}

// We need a base type, either a class or an interface.
public interface IData
{
    // All children must have a parent Id
    // that refers to SomeParent in this case.
    int ParentId { get; set; }
}

// Finally, one of the subclasses.
public class SomeData : IData
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string SomeProperty { get; set; }
}
```

Take advantage of this structure by moving all common columns/properties into the ChildConnector class and then have only sub-class-specific data in the sub-classes.

The mappings look like normal one-to-many mappings, but with a discriminator configuration:

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

internal class ChildConnectorMapping : IEntityConfigurator<ChildConnector>
{
    public void Configure(IClassMappingConfigurator<ChildConnector> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        // subclass configuration is activated by the
        // discriminator.
        config.HasOne(x => x.Data)
              .Discriminator(x => x.DataType, ChildSelector) 
              .ForeignKey(x => x.ParentId)
              .References(x => x.Id);

        config.MapRemaningProperties();
    }

    private Type? ChildSelector(string arg)
    {
        return arg switch
        {
            "SomeData" => typeof(SomeData),
            "MoreData" => typeof(Moredata),
            _ => null
        };
    }    
}


internal class SomeChildMapping : IEntityConfigurator<SomeChild>
{
    public void Configure(IClassMappingConfigurator<SomeChild> config)
    {
        config.Key(x => x.Id).AutoIncrement();
        config.MapRemaningProperties();
    }
}
```

The tables are the same, except for the junction table and updated foreign keys.

```sql
create table SomeParents
(
    Id int not null identity primary key
)

create table ChildConnector
(
    Id int not null identity primary key,
    SomeParentId int not null constraint FK_SomeParentId_SomeParents foreign key references SomeParents(Id)
    DataType varchar(40) not null
)

create table SomeChildren
(
    Id int not null identity primary key,
    SomeParentId int not null constraint FK_SomeParentId_SomeParents foreign key references SomeParents(Id)
    SomeProperty varchar(40) not null
)
```

## Limiting rows for child entities

Sometimes you want to get only specific rows of child entities.

Here is an example:

```csharp
public class MainClass
{
    public IReadOnlyList<SubClass> LeftOptions { get; set; }
    public IReadOnlyList<SubClass> RightOptions { get; set; }
}
```

To achieve that, configure a SubsetColumn. 

```sql
create table SubClass
(
    Id int not null,
    Option varchar(40) not null,
    SomeOtherProperty int not null
)
```

The contents of the column will be managed by the library (i.e., it's not added to the classes).

```csharp
internal class MainClassMapping : IEntityConfigurator<MainClass>
{
    public void Configure(IClassMappingConfigurator<MainClass> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.HasMany(x => x.LeftOptions)   // Child property in the arent class.
            .SubsetColumn("SubClass", "Left")
            .ForeignKey(x => x.ParentId)  // FK property in the child class (SomeChild).
            .References(x => x.Id);       // property in the main entity (SomeParent) that the FK references.
            
        config.HasMany(x => x.RightOptions)   // Child property in the arent class.
            .SubsetColumn("SubClass", "Right")
            .ForeignKey(x => x.ParentId)  // FK property in the child class (SomeChild).
            .References(x => x.Id);       // property in the main entity (SomeParent) that the FK references.
            
        config.MapRemaningProperties();
    }
}
```

## Restrictions

The following restrictions apply to has-many mappings.

### Must be an interface

The collection property must be an interface and not a concrete type.

The following interfaces are supported:

* `IEnumerable<T>`
* `IReadOnlyList<T>`
* `IList<T>`
* `T[]` (arrays)
