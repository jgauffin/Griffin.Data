Database support
================

Currently, only Microsoft SQL Server is supported. However, it's easy to add support for more DB engines.

Here is a guide:

1. Add a new GitHub issue and tag it `extending`. Tell which DB engine that you are building support for. 
2. Create a new class library, `Griffin.Data.DbEngineName`, .NET target frameworks should be at minimum "NetStandard2.0" and "net60". Nullable should be enabled in the csproj.
3. Optionally define a nuget package in the csproj (or we'll do that for you).
4. Create a new class, name it for instance `PostgresDialect`, and implement `ISqlDialect`.
5. Implement the class.
6. Create a test project and copy all tests from `Griffin.Data.Mssql.Tests` and make sure that they work with your library.
7. Create a pull request and link it to your github issue.

## Implementation details

This is what your new class should do.

### INSERT method

The `Insert` method must fetch the value from an auto-increment column and set it in the correct property in the entity.
Generates/Sequences should be called before INSERT if required.

### Query method

Should apply sorting, paging and limit to the SQL statement from the `SqlOption` object.

### Open connection method

Should create a new connection, open it using the supplied connection string, and return it once open.

