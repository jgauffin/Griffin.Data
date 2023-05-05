Queries
=======

Use queries to generate aggregated information from one or multiple tables, as example for the read side of CQRS.

Build the query classes manually, or use the recommended approach: The scaffolder in this library takes SQL query files and generates classes based on them.

# Create a new query.

Here is an example of the process of generating queries.

In this case, we'll create a query that returns all active users.

## Write a SQL query

Use SQL Management Studio (or your favorite SQL IDE) and write a nice query.

Here is a minimal example:

```sql
SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE a.State = 1;
```

## Save the query in a file in your project

Copy/paste the query into a new script in your database project.

It can be named `ListActiveUsers.query.sql`:

![](solution-explorer.png)

The `.query.sql` part is important as it tells the scaffolder to generate a new script.

## Run the scaffolder

Open a command prompt or powershell and run the scaffolder (install it first using `dotnet tool install -g griffin.data.scaffolding`).

```
dotnet gd queries
```

It will now generate all files.

![](solution-explorer.png)

## View the result

There is now a couple of classes in the same folder as the query script.

```csharp
// It's empty since no parameters were supplied. 
// It does, however, specify the type it will return, `ListActiveUsersResult`.
public class ListActiveUsers : IQuery<ListActiveUsersResult>
{
}

// The result returned from the query handler.
public class ListActiveUsersResult
{
    public IReadOnlyList<ListActiveUsersResultItem> Items { get; set; }
}

// The result item with details.
public class ListActiveUsersResultItem
{
    public int Id  { get; set; }
    public string UserName  { get; set; }
}

// The query handler (which executes the query and generates the result).
//
// Since query handlers use ADO.NET directly, they are as fast as it gets.
// (there are a few optimisations left to do, but it should be fast enough as is)
public class ListUsersHandler : ListHandler<ListUsersResultItem>, IQueryHandler<ListUsers, ListUsersResult>
{
    public ListUsersHandler(QuerySession session) : base(session)
    {
    }

    public async Task<ListUsersResult> Execute(ListUsers query)
    {
        await using var command = Session.CreateCommand();
        command.CommandText = @"SELECT u.Id, UserName
                                FROM Users u
                                JOIN Accounts a ON (u.AccountId = a.Id)
                                WHERE a.State = 1";

        command.AddParameter("name", query.NameToFind);
        return new ListUsersResult { Items = await MapRecords(command) };
    }

    // This map method is as fast as ADO.NET gets.
    protected override void MapRecord(IDataRecord record, ListUsersResultItem item)
    {
        item.Id = record.GetInt32(0);
        item.UserName = record.GetString(1);
    }
}
```

The generated query uses regular ADO.NET to avoid mapping issues or inefficiencies. 

## Update or regenerate queries

The scaffolder will not modify existing files. To get a query re-generated, delete it and invoke the scaffolder again.

You can also invoke `dotnet gd queries regenerate` which will overwrite all queries.

## Using query parameters

Query parameters in the SQL script are automatically converted into query properties.

```sql
declare @nameToFind varchar(40) = 'TestName';

SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE u.UserName LIKE @name
```

The above query results in

```csharp
public class ListActiveUsers : IQuery<ListActiveUsersResult>
{
    public string NameToFind { get; set; }
}
```

## Paging

To use paging, simply add `--paging` to the first line of the SQL script.

```sql
--paging
declare @nameToFind varchar(40) = 'TestName';

SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE u.UserName LIKE @name
```

```csharp
public class ListActiveUsers : IQuery<ListActiveUsersResult>
{
    public string NameToFind { get; set; }
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 50;

    public void Page(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) 
        {
            throw new ArgumentExcpetion("PageNumber is a one-based index");
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
```

## Sorting

To allow the query invoker to sort the result, add `--sorting` to the top of the query.

```sql
--sorting
--paging
declare @nameToFind varchar(40) = 'TestName';

SELECT u.Id, UserName
FROM Users u
JOIN Accounts a ON (u.AccountId = a.Id)
WHERE u.UserName LIKE @name
```

```csharp
public class ListActiveUsers : IQuery<ListActiveUsersResult>
{
    private List<QuerySort> _sorting = new List<QuerySort>();

    public string NameToFind { get; set; }
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 50;
    public IReadOnlyList<QuerySort> Sorting { get { return _sorting; } private set { _sorting = new List<QuerySort>(value); } }

    public ListActiveUsers Page(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0) 
        {
            throw new ArgumentExcpetion("PageNumber is a one-based index");
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
        return this;
    }
    
    public ListActiveUsers SortAscending(string propertyName)
    {
        _sorting.Add(new QuerySort(propertyName, true));
        return this;
    }
    public ListActiveUsers SortDescending(string propertyName)
    {
        _sorting.Add(new QuerySort(propertyName, false));
        return this;
    }
}
```

Use the generated query like this:

```csharp
var query = new ListActiveUsers('jo%")
    .Page(2, 20)
    .SortAscending("Id")
    .SortDescending("Name");

var result = await Session.Query(query);
```

## Configuration

Queries are run either by using an DI container or by using an extension method for `Session`.

### Using IOC

Using an IoC is our recommended approach. There is a `QuerySession` class which inherits the normal `Session` class. That allows us to use different database transaction isolation levels for queries and commands. That's a good thing since queries aren't typically as strict when it comes to data (as it's just presented to users). 

You can for instance use "read committed" for queries while using "serializable" for the normal session. You can find details about isolations levels in the [microsoft docs](https://learn.microsoft.com/en-us/dotnet/api/system.data.isolationlevel).

Register as follows:

```csharp
public void RegisterQueries(IServiceCollection services)
{
    services.AddQueryHandlerInvoker(IsolationLevel.ReadCommited);

    // Repeat this line for all assemblies that contain IQueryHandler<,> implementations.
    servives.AddQueryHandlers(typeof(SomeQueryHandler).Assembly);
}
````

### Using Session

If you want to use `Session.Query()` you need to instead register all assemblies in `SelfContainedQueryHandlerInvoker`:

```csharp
// Repeat per assembly that contains query handlers.
SelfContainedQueryHandlerInvoker.ScanAssembly(typeof(MyQueryHandler).Assembly);
```


## Invoke queries

When using DI:

```csharp
public class AccountController
{
    private readonly IQueryHandlerInvoker _invoker;

    public AccountController(IQueryHandlerInvoker invoker)
    {
        _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
    }

    public Task<IReadOnlyList<TrialAccountResult>> LockTrialAccounts()
    {
        return await _invoker.Execute(new FindTrialAccounts());
    }
}
```

When using the `Session` class:

```csharp
var result = await Session.Query(new ListActiveUsers());
```
