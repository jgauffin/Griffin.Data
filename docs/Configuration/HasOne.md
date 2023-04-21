One to zero/one
==============

One-to-one means that one class has a single child entity. 

```csharp
public class User
{
    public Address Address { get; set; }
}
```

Children may also be nullable:

```csharp
public class User
{
    public Address? Address { get; set; }
}
```


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

        config.HasOne(x => x.Children)    // Child property in the parent class.
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

Inheritance is easy to configure. First, add two properties in your parent class, one to specify which child type and one for the child. The "child type" property can consist of anything, as you use it, and not the library, to determine the child type.


```csharp
public class SomeParent
{
    public int Id { get; set; }

    // This property is used to load the correct type
    // The type of field doesn't matter for the library.
    // enum, string, or int are the most common.
    public string ChildType { get; set; }

    // The correct child is loaded into this property.
    public ISomeChildBaseType Child { get; set; }
}

// We need a base type, either a class or an interface.
public interface ISomeChildBaseType
{
    // All children must have a parent Id
    // that refers to SomeParent in this case.
    int ParentId { get; set; }
}

// Finally, one of the sub classes.
public class SomeData : ISomeChildBaseType
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string SomeProperty { get; set; }
}
```

The mappings look like regular one-to-one mappings, but with a discriminator configuration:

```csharp
internal class SomeParentMapping : IEntityConfigurator<SomeParent>
{
    public void Configure(IClassMappingConfigurator<SomeParent> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        // subclass configuration is activated by the
        // discriminator.
        config.HasOne(x => x.Data)
              .Discriminator(x => x.ChildType, ChildSelector) 
              .ForeignKey(x => x.ParentId)
              .References(x => x.Id);

        config.MapRemaningProperties();
    }

    private Type? ChildSelector(string arg)
    {
        // Could have been an int or en enum too
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

The tables are the same except for the extra field in the parent table.

```sql
create table SomeParents
(
    Id int not null identity primary key
    ChildType varchar(40) not null
)

create table SomeChildren
(
    Id int not null identity primary key,
    SomeParentId int not null constraint FK_SomeParentId_SomeParents foreign key references SomeParents(Id)
    SomeProperty varchar(40) not null
)
```

## Limiting rows for child entities

Sometimes you want to get only specific rows of the child entities.

Here is an example:

```csharp
public class MainClass
{
    public SubClass LeftOptions { get; set; }
    public SubClass RightOptions { get; set; }
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

The contents of the column will be managed by the library (i.e. it's not added to the classes).

```csharp
internal class MainClassMapping : IEntityConfigurator<MainClass>
{
    public void Configure(IClassMappingConfigurator<MainClass> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.HasOne(x => x.LeftOptions)   // Child property in the arent class.
            .SubsetColumn("SubClass", "Left")
            .ForeignKey(x => x.ParentId)  // FK property in the child class (SomeChild).
            .References(x => x.Id);       // property in the main entity (SomeParent) that the FK references.
            
        config.HasOne(x => x.RightOptions)   // Child property in the arent class.
            .SubsetColumn("SubClass", "Right")
            .ForeignKey(x => x.ParentId)  // FK property in the child class (SomeChild).
            .References(x => x.Id);       // property in the main entity (SomeParent) that the FK references.
            
        config.MapRemaningProperties();
    }
}
```
