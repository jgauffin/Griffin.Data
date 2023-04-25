Let's face it. SQL is easy to learn. There are fantastic tools to craft queries and ensure that they are efficient. If you are good at SQL, you can visualize a query's efficiency and which indexes you must create.

On the other hand, LINQ To SQL is a layer of indirection. It tries to solve a problem that isn't there. It teaches you to write SQL in a new way, and it's more challenging to understand what the generated query will look like than reading an explicit query. Therefore, you must master two different query languages to write efficient database calls.

But the worst part is that most LINQ To SQL providers are pretty complex because only the most trivial queries are easily translated to SQL.

Ask yourself, what do you get by adding another query language? Is the additional complexity worth it? The extra type safety that LINQ to Sql provides is only isolated to the database layer, which either way must be integration tested to ensure that all mappings are correct.

I think that the bulk update feature in EF core is a perfect example:

```csharp
await context.Users
             .Where(x => x.Id > 1000)
             .ExecuteUpdateAsync(x => x.SetProperty(x => x.Category, x => "Customer"));
```

Instead of:

```csharp
await connection.Execute("UPDATE Users SET Category = 'Customer' WHERE Id > 1000");
```

In Griffin.Data, I've made an active choice not to support LINQ. Instead, you'll write SQL queries and get code generated based on those queries for you. The queries, and their result, are still type-safe but much more efficient and without any extra complexity. What you see is what you get.

That works fine for the read side, but the ORM uses plain SQL. I do regognize that compilation errors do add some value, even though that all mappings need to be integration tested. That's why I'm working on a roslyn analyzer which will test all SQL statements against the mappings (and by doing so be able to add compile time errors by parsing the SQL).