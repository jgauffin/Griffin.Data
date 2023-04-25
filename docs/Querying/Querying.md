Querying
========

This information is regarding querying business entities and not for using read-side queries.

Querying lets you fetch single or multiple entities of the same type. 

Query methods:

* `First()` - Fetch an entity, or throw a detailed exception if no entity is found.
* `FirstOrDefault()` - Try to find an entity, or default <c>null</c> if none are found.
* `GetById()` - Get a specific entity (works only for entities which have a single column primary key).
* `List()` - Get a collection of rows.

All below examples work with any of the above methods:


```csharp
// Queries can either use only parameters (where the parameter names must be property names):
var users = session.First<User>(new { UserId });
```


```csharp
// Short form SQL query:
var users = session.List<User>("user_id = @userId", new { userId });
```


```csharp
// complete SQL statements:
var users = session.FirstOrDefault<User>("SELECT * FROM Users WHERE user_id = @userId", new { userId });
```

When using query options, you can instead invoke them directly on the session:

```csharp
var result = await session.Query<MainTable>()
    .Where(new { FirstName = "Jonas" })
    .DoNotLoadChildren()
    .Paging(2, 10)
    .OrderByDescending(x => x.Age)
    .List();
```

## Query rules

There is a simple rule to follow when making queries.

When using parameters only, the key must be property names:

```csharp
var users = session.First<User>(new { UserId });
```

When using a SQL statement (complete or short form), keys must be the same as in the SQL query:

```csharp
var users = session.First<User>("user_id = @userId", new { userId });
```

SQL statements can be just the WHERE clause, as in the example above, or a complete SQL statement.

## Getting a single entity

For entities with a single key column,  use the `GetById<T>()` method.

```csharp
var user = await session.GetById<User>(userId);
```

You can also fetch it by using a combination of properties:

```csharp
var user = await session.First<User>(new { userId, state });
```

That works when the variables have the same names as the properties in the `User` class.

You can also specify a SQL WHERE statement:

```csharp
var user = await session.First<User>("user_id = @userId AND state <> @state", new {userId, state});
```

Or a complete SQL statement:

```csharp
var user = await session.List<User>("SELECT * FROM Users WHERE Id = @id", new { id = userId });
```

To avoid loading children, toggle that:

```csharp
var user = await session.Query<User>(new { id = userId })
            .DoNotLoadChildren()
            .First();
```

## Getting multiple entities

Fetch multiple entities works in the same way:

```csharp
var users = await session.List<User>(new { FirstName = "J%" });
```

Do note that when you use `%`, the library will automatically generate a LIKE statement in the WHERE clause.

You can also use complete SQL statements or short form (only the contents of WHERE).

## Paging and sorting

```csharp
var users = await session.Query<User>()
     .Where(new { FirstName = "J%"})
     .Paging(2, 10); // Go to page 2, there are 10 items per page.
     .Ascending(x => x.LastName); // first sort by last name.
     .Ascending(x => x.FirstName); // then by last name.
     .List();
```
