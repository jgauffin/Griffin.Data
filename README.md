Griffin.Data
============

***This library hass been merged into [Griffin.Framework](https://github.com/jgauffin/griffin.framework) - Got a lot more functions in Griffin.Framework. ***



Simple Data Mapping Layer

The purpose of this layer is not to create an OR/M but to help you map the result of your SQL Queries to your domain entities (or POCOs).

# Mapping only

You start by creating a mapping:

    public class UserMapping : SimpleMapper<User>
    {
        public UserMapping()
        {
            Add(x => x.Id, "Id");
            Add(x => x.FirstName, "FirstName");
            Add(x => x.LastName, "LastName");
            Add(x => x.Age, "Age");
            Add(x => x.CreatedAt, "CreatedAt", new SqlServerDateConverter());
        }
    }
	
Which you register:

	MapperProvider.Instance.RegisterAssembly(Assembly.GetExecutingAssembly());

Then you create methods like:

	public IEnumerable<User> FindAll()
	{
		using (var cmd = _connection.CreateCommand())
		{
			cmd.CommandText = "SELECT * FROM Users";
			
			// this is the magic
			return cmd.ExecuteQuery<User>();
		}
	}

Done!

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

# Access helpers

Makes it a bit easier to fetch stuff. Remember. Still not an OR/M but just a simple access layer.

Create a mapping (and register it like before):

	public class UserMapping : EntityMapper<User>
    {
        public UserMapping()
        {
            Add(x => x.Id, "Id");
            Add(x => x.FirstName, "FirstName");
            Add(x => x.LastName, "LastName");
            Add(x => x.Age, "Age");
            Add(x => x.CreatedAt, "CreatedAt", new SqlServerDateConverter());

			// the new stuff
            TableName = "Users";
            IdColumnName = "Id";
        }
    }
	
And then fetch one item by doing:

	using (var cmd = _connection.CreateCommand())
	{
		return cmd.ExecuteScalar<User>(id);
	}
	
Fetch several (all params are AND:ed)

    using (var cmd = _connection.CreateCommand())
    {
		// note that it's property names and not column names.	
		return cmd.ExecuteLazyQuery<User>(new { FirstName = "Arne", LastName = "Kalle" }).FirstOrDefault();
    }

Easy paging (no need to care about the db server implementation):

	var context = new DbPagerContext(sql, constraints.PageNumber, constraints.PageSize);
	var pager = new SqlServerCePager();
	sql = pager.ApplyTo(context);
