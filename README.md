Griffin.Data
============

Simple Data Mapping Layer

The purpose of this layer is not to create an OR/M but to help you map the result of your SQL Queries to your domain entities (or POCOs).

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
			var result = cmd.ExecuteQuery<User>();
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
			// notice the SQL pager class which transforms the SQL
			var pager = new SqlServerPager();
			sql = pager.ApplyTo(sql, constraints.PageNumber, constraints.PageSize, "id");
		}

		return sql;
	}
}