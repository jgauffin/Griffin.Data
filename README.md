Griffin.Data
============

Version 2.0 is work in progress and not ready for use.

Lightweight ORM and data mapper.

The ORM part is inteded to manage business entites (no joins etc) while the data mapper is used to build queries for the view part of an application.

Licence: Apache License v2.0

# ORM


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
var user = dbScope.GetById<User>(1);
user.LockAccount();
dbScope.SaveChanges();
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
