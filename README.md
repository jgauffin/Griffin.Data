Griffin.Data
============

Version 2.0 is work in progress and not ready for use.

Lightweight ORM and data mapper.

The ORM part is inteded to manage business entites which means that joins or fetching specific columns is not supported.
However, there is also a data mapper included which can generate custom queries (and mappings) from your custom SQL queries.

Licence: Apache License v2.0

# ORM

## Features

* Change tracking (currently through snapshots, change proxies are being developed).
* DB independing handling of paging, sorting and to limit the number of rows.
* One to many and many to one.
* Inheritance support.
* Minimal mapping configuration.
* 

## Features that never will be implemented

Our goal is to create a ORM which is easy to use and to debug. Therefore, we have not, and will not, implement the following features:

* LINQ - You need to write SQL (partial or complete statements) or use constraints like `var user = await session.First<User>(new { firstName = 'Jonas' })`.
* Lazy loading - The internal loading strategies promote bulk fetches for children. If that's not enough, you are probably doing something wrong.



Start by creating a mapping:

```csharp
internal class UserConfigurator : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.FirstName);
        
        config.HasMany(x=>x.Addresses)
            .ForeignKey(x=>x.UserId)
            .References(x=>x.Id);
        
        config.HasOne(x=>x.Data)
            .Denominator(x=>x.State, CreateChildEntity)
            .ForeignKey(x=>x.UserId)
            .References(x=>x.Id);
        
    }

    private Data? CreateChildEntity(AccountState arg)
    {
        switch (arg)
        {
            case AccountState.Active:
                return new UserData();
            case AccountState.Admin:
                return new AdminData();
            default:
                return null;
        }
    }
}
```

There is scaffolding included which can generate both entities (parent and child entities using foreign keys) and mappings for those.

All entities are change tracked and only those changed are persisted back to the database.

Next, use the DbScope to apply changes:

```csharp
var user = session.GetById<User>(1);
user.LockAccount();
session.SaveChanges();
```

# Queries

There is also a small query framework which allows you to query like:

	var constraints = new QueryConstraints<User>()
		.SortBy(x => x.FirstName)
		.Page(2, 50);
	var result = queries.FindAll(constraints);

	foreach (var user in result.Items)
	{
		// Note that each user is not mapped until it's requested
		// as opposed to the entire collection being mapped first.
		Console.WriteLine(user.FirstName);
	}

Where the actual implementation looks like (all used classes exist in Griffin.Data):

	public IQueryResult<User> FindAll(IQueryConstraints<User> constraints)
	{
		using (var cmd = _connection.CreateCommand())
		{
			cmd.CommandText = "SELECT * FROM Users";

			// count
			var count = (int)cmd.ExecuteScalar();

			// page
			cmd.CommandText = ApplyConstraints(constraints, cmd.CommandText);
			var result = cmd.ExecuteLazyQuery<User>();
			return new QueryResult<User>(result, count);
		}
	}

	private static string ApplyConstraints(IQueryConstraints<User> constraints, string sql)
	{
		if (!string.IsNullOrEmpty(constraints.SortPropertyName))
		{
			sql += " ORDER BY " + constraints.SortPropertyName;
			if (constraints.SortOrder == SortOrder.Descending)
				sql += " DESC";
		}

		if (constraints.PageNumber != -1)
		{
			var context = new SqlServerPagerContext(sql, constraints.PageNumber, constraints.PageSize, "Id");
			var pager = new SqlServerPager();
			sql = pager.ApplyTo(context);
		}

		return sql;
	}
}
