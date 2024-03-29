Griffin.Data
============

<img src="https://ci.appveyor.com/api/projects/status/github/jgauffin/griffin.data?branch=master&svg=true" />
<img src="https://img.shields.io/nuget/dt/griffin.data">
<img src="https://img.shields.io/nuget/v/griffin.data">
<img src="https://img.shields.io/tokei/lines/github/jgauffin/Griffin.Data">
<img src="https://img.shields.io/github/license/jgauffin/Griffin.Data">

Version 2.0 is a work in progress; most features are in place but still need to be completed.

## Why Griffin.Data?

1. With Griffin.Data, you can maintain a high degree of separation between your application and data layer, ensuring that your domain entities remain protected and free from unnecessary dependencies or complexities.

2. Griffin.Data reduces the time and effort required to set up and configure data access. With this library, you can quickly and easily create a robust data layer that can handle complex queries and data operations without worrying about compatibility issues or complex configuration.

3. This library provides detailed and easy-to-understand error messages, enabling you to quickly identify and address the root cause of any issues that arise. No need for time-consuming trial and error or extensive debugging efforts. 


## What is it?

Griffin.Data consists of a lightweight object/relation mapper (ORM) and data mapper.

This library assumes that the write part of an application (changing state) is fundamentally different from the read part (presenting information to the user).

### The write side

The write site usually fetches one or more business entities (domain entities), modifies them, and persists in their changes. An ORM is perfect for this as all entities are well-defined, and each entity usually represents a single table in the database.

This ORM does not require any changes to your entities (no setters are necessary). A scaffolder can generate everything needed (entities, mappings etc). Collection properties can even be IReadOnlyList<YourChildEntity> to protect the state in your entities.

Highlights:

* Change tracking (no need to manually call create/update/delete)
* Relationships (one to many, one to one)
* Inheritance support
* Built in enum support (can be represented by byte, short, int and strings in the database).
* Setters are not required
* Child entity collections can be IList<T>, IReadOnlyList<T> or arrays.
* Detailed exceptions to make debuggin a breaze.

### The read side

Presenting information to users usually requires a join of data from different tables or aggregated queries; there are as many queries as views shown for the user. Because of that, there is no simple way to produce classes and mappings as on the write side.

This library lets you define (simple or complex) SQL queries and generates classes based on them. The generated classes can easily be regenerated when you update any of the queries.

The following query, `ListUsers.query.sql` will generate a query, a result class, and a query runner.


```sql
--sorting
--paging
declare @nameToFind varchar(40) = 'TestName';

SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE u.UserName LIKE @nameToFind
```

Use scaffolding to generate classes:

```
dotnet gd queries
```

Execute queries in the following way:

```csharp
var result = await Session.Query(new ListUsers { NameToFind = 'A%'});
Console.WriteLine($"Found {result.Items.Count} users.");
```

Highlights:

* `--sorting` will add sort options to the query
* `--paging` will add page options to the query
* Adds a query property based on the SQL parameter.
* Possible to use the query class as a DTO in an API (i.e. JSON friendly).
