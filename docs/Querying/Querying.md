Querying
========

This information is regarding querying business entities and not for using read-side queries.

Querying lets you fetch single or multiple entities of the same type. 

Queries can either use only parameters (where the parameter names must be property names):

```csharp
var users = session.First<User>(new { UserId });
```

Short form SQL query:

```csharp
var users = session.First<User>("user_id = @userId", new { userId });
```

Or complete SQL statements:

```csharp
var users = session.First<User>("SELECT * FROM Users WHERE user_id = @userId", new { userId });
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

Paging and sorting can be taken care of by this library to apply them in a DB engine-specific way.

## Getting a single entity

For entities with a single key column,  use the `GetById<T>()` method.

```csharp
var user = await session.GetById<User>(userId);
```

You can also fetch it by using a combination of properties:

```csharp
var user = await session.First<User>(QueryOptions.Where(new {userId, state}));
```

That works when the variables have the same names as the properties in the `User` class.

You can also specify a SQL WHERE statement:

```csharp
var user = await session.First<User>(QueryOptions.Where("user_id = @userId AND state <> @state", new {userId, state}));
```

Or a complete SQL statement:

```csharp
var query =  new QueryOptions("SELECT * FROM Users WHERE Id = @id", new { id = userId });
var user = await session.First<User>(query);
```

To avoid loading children, toggle that:

```csharp
var query = new QueryOptions<User>
            .Where(new { id = userId })
            .DoNotLoadChildren();

var user = session.First(query);
```

## Getting multiple entities

Fetch multiple entities works in the same way:

```csharp
var users = await session.List<User>(QueryOptions.Where(new {FirstName = "J%"}));
```

Do note that when you use `%`, the library will automatically generate a LIKE statement in the WHERE clause.

You can also use complete SQL statements or short form (only the contents of WHERE).

## Paging and sorting

```csharp
var query = new QueryOptions<User>
     .Where(new {FirstName = "J%"})
     .Paging(2, 10); // Go to page 2, there are 10 items per page.
     .Ascending(x => x.LastName); // first sort by last name.
     .Ascending(x => x.FirstName); // then by last name.

var users = await session.List(query);
```
