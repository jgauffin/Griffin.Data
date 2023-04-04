Querying
========

Querying lets you either fetch a single or multiple entities of the same type. 

## Query rules

There are some rules that must be obeyed in queries:

* Constrains (`QueryOptions.Where(new { userId })`) must always use property names.
* SQL statements must always use column names.

Example:

```csharp
var users = session.First<User>("user_id = @userId", new { userId });
```

## Getting a single entity

If you have a single key column you can use the `GetById<T>()` method:

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
var user = await session.First<User>(QueryOptions.Where("UserId = @userId AND State <> @state", new {userId, state}));
```

Or a complete SQL statement:

```csharp
var query =  new QueryOptions("SELECT * FROM Users WHERE Id = @id", new { id = userId });
var user = await session.First<User>(query);
```

To avoid loading children, toggle that:

```csharp
var query =  new QueryOptions(new { id = userId });
query.LoadChildren = false;
var user = await session.First<User>(query);
```

## Getting multiple entities

Fetch multiple entities works in the same way:

```csharp
var users = await session.List<User>(QueryOptions.Where(new {FirstName = "J%"}));
```

Do note that when you use `%`, the library will automatically generate a LIKE statement in the WHERE clause.

You can also use complete SQL statements or short form (only the contents of WHERE).

To limit result or paging:

```csharp
var query = QueryOptions<User>.Where(new {FirstName = "J%"});
query.Paging(2, 10); // Go to page 2, there are 10 items per page.
query.Ascending(x => x.LastName); // first sort by last name.
query.Ascending(x => x.FirstName); // then by last name.

var users = await session.List<User>(query);
```
