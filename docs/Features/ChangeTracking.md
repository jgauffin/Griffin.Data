Change tracking
===============

The current implementation uses snapshots to detect changes. Snapshots mean the library internally creates a copy of each fetched entity and stores it in memory until `Session.SaveChanges()` is called. Once `Session.SaveChanges()`  are called, the library generates a diff between the snapshot (i.e. the copy of your entity) and the modified entity. The diff contains added/modified/removed entities. Those changes will be persisted (without explicit calls to Insert/Update/Delete). Entities without changes will be ignored (i.e. no database calls will be made for them).

As this implementation uses snapshots, the memory footprint of each `Session` is doubled, but in turn, you don't have to modify your entities in any way. Keep in mind that the intended purpose of the object/relation mapper in this library is to manage business entities (i.e. change the state of your application). Thus, the memory footprint is usually small for the write side of your application.

 ## Configuration

 Change tracking is disabled by default. Turn it on by adding `SnapshotChangeTracking` to your IoC container:

 ```csharp
 services.AddScoped<IChangeTracker, SnapshotChangeTracking>();
 ```

 You can also add it directly to the `DbConfiguration`:

 ```csharp
 var connectionString = config.GetConnectionString("Db");

 var config = new DbConfiguration(connectionString)
 {
    SqlDialect = new SqlServerDialect();
 };
config.ChangeTrackerFactory = () => new SnapshotChangeTracking(config.MappingRegistry);
 ```
 

 ## Checking state

To check the state of an entity, invoke `changeTracker.GetState(yourEntity);`. 

Before checking state of child entities, call `changeTracker.Refresh(rootEntity)` as added/removed state won't be detected otherwise.

 ## Current limitations

 * The snapshot can only contain a single copy of each entity. Therefore, if you invoke queries for the same entity multiple times, only the last fetched instance will be change tracked. That also applies to child entities. 