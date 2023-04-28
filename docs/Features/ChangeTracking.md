Change tracking
===============

Change tracking means that the data library keeps track of all entitiesa that have been added, modified or removed. There are two more common approaches to change tracking. 

The first one is using a snapshot. That means that every time you fetch an entity from the database, a copy of it is made. The copy is kept internally and is not used until you invoke `SaveChanges()`. Then the copy is then compared to your modified entity to find all made changes. The comparison is done throughout the object hierarchy and all changes are persisted by regular insert/update/delete SQL commands. Entities without changes are ignored (i.e. no database calls will be made for them).

Change tracking requires no changes to your entities, but requires a bit more memory than proxiesa and the actual comparison can get complex within the library.

The second approach is to wrap the real entity in proxy and then return the proxy from the library get methods. Every change is made upon the proxy, which logs the changes and then apply them to the real object. The good thing is that it's pretty lightweight, but it requires that all properties and methods in your entities are virtual.

Griffin.Data uses snapshots to track changes.

## Configuration

Change tracking is disabled by default. Turn it on by adding `SnapshotChangeTracking` to your IoC container:

```csharp
services.AddScoped<IChangeTracker, SnapshotChangeTracking>();
```

You can also add it directly to the `DbConfiguration`:

```csharp
var connectionString = config.GetConnectionString("Db");

var dbConfig = new DbConfiguration(configurationString)
    .AddMappingAssembly(typeof(TodoTaskMapping).Assembly)
    .UseSnapshotChangeTracking() // this line
    .UseSqlServer();
```

## Checking state

The change tracking is fully automated and there is no need to modify the state or manually apply changes.

But sometimes you may want to check the state. That is done by invoking `changeTracker.GetState(yourEntity);`. Do note that the change tracker got one instance per session. To be able to access it you need to have registered it in the IoC container.

Before checking state of child entities, call `changeTracker.Refresh(rootEntity)` as added/removed state won't be detected otherwise.

## Change tracking for externally manage entities

In some applications it makes sense to modify entities externally (like in a client application or in a web application). Change tracking is in that case not possible as the modification is not made in the tracked entities.

You can handle that by doing the comparison manually. There is a class called `SingleEntityComparer` which compares a single entity (and all it's children).

You can use it as following:

```csharp
var dbVersion = _session.GetById<SomeEntity>(externalCopy.Id);

var service = new SingleEntityComparer();
var result = service.Compare(dbVersion, externalCopy);
```

That comparison will give you a diff which tells you all the entities that have been changed.

You can either apply it manually by traversing the result and call Insert/Update/Delete, or use the `ChangePersister` class.

```csharp
var persister = new ChangePersister(_mappingRegistry);
await persister.Persist(_session, result);
await session.SaveChanges();
```

Once done, all changes should have persited.

 ## Current limitations

 * The snapshot can only contain a single copy of each entity. Therefore, if you invoke queries for the same entity multiple times, only the last fetched instance will be change tracked. That also applies to child entities. 
