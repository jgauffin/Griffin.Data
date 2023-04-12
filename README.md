Griffin.Data
============

Version 2.0 is work in progress, most features are in place but not fully completed.

Lightweight object/relation mapper (ORM) and data mapper.

This library is based on the assumption that the write part of an application (changing state) is fundamentally different from the read part (presenting information from the user).

## The write side

The write site usually fetches one or more buinsess entities (domain entities), modify them and then persist their changes.
A ORM is perfect for this as all entities are well defined and each entity is usally represented by a single table in the database.

This ORM does not require any changes to your entities (no setters are required). There is a scaffolder that can generate everything required (entities, mappings etc).
Collection properties can even be `IReadOnlyList<YourChildEntity>` to protect the state in your entities.

Highlights:

* Relationships (one to many, one to one)
* Inheritance support
* Built in enum support (can be represented by byte, short, int and strings in the database).
* Setters are not required
* Child entity collections can be  `IList<T>`, `IReadOnlyList<T>` or arrays.

## The read side.

Presenting information to users usally requires a join if information from different tables or aggregated queries. Since each view is unique there is not a simple way to produce classes and mappings as in the write side.

This library lets you define SQL queries and generates classes based on them. These classes can easily be regenerated when you change the query.

The following query, `ListUsers.query.sql` will generate a query, a result class and a query runner.


```sql
--sorting
--paging
declare @nameToFind varchar(40) = 'TestName';

SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE u.UserName LIKE @name
```

Use scaffolding to generate classes:

```
dotnet gf generate queries
```

A query can be executed like this:

```csharp
var result = await Session.Query(new ListUsers { FirstName = 'A%'});
Console.WriteLine("Found " + result.Items.Count  + " users.");
```

Highlights:

* `--sorting` will add sort options to the query
* `--paging` will add page options to the query
* The SQL parameter will be added as a property in the query.
* The query class is simple and can be used as a DTO in an API (i.e. JSON friendly).
