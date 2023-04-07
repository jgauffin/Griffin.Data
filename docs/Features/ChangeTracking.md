Change tracking
===============

The current implementation uses snapshots to detect changes. Snapshots means that the library internally creates a copy of each fetched entity and stores it in memory until `Session.SaveChanges()` are called. Once `Session.SaveChanges()`  are called, the library generates a diff between the snapshot (i.e. the copy of your entity) and the entity that was modified. The diff contains added/modified/revmoed entities. Those changes will be persisted (without explicit calls to Insert/Update/Delete). Entities that have not been modified will be ignored (i.e. no database calls will be made for them).

As this implentation uses snapshots, the memory footprint of each `Session` is doubled, but in turn you don't have do modify your entities in any way. Keep in mind that the intended purpose of the object/relation mapper in this library is to manage business entities (i.e. change the state of your application). Thus, the memory footprint is usally small for the write side of your application.

 ## Configuration

 Change tracking is disabled by default. Turn it on by adding `SnapshotChangeTracking` to your IoC container:

 ```csharp
 services.AddScoped<IChangeTracker, SnapshotChangeTracking>();
 ```

 ## Checking state

State can be checked by invoking `changeTracker.GetState(yourEntity);`. 

To check state of child entities, invoke  `changeTracker.Refresh(rootEntity)` first, as added/removed state won't be detected otherwise.

 ## Current limitations

 * The snapshot can only contain a single copy of each entity. If you invoke queries for the same entity multiple times, only the last fetched instance will be change tracked. That also applies for child entities. 