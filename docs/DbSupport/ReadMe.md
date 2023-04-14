Database support
================

Currently, only Microsoft SQL Server is supported. However, each database engine only requires a minimal implementation.

# Walkthrough

These are the steps. 

## Create a github issue

To prevent that the same engine gets multiple implementations, create a github issue. It will also be used by you to ask questions too.

## Create a new class library.

This step is required to reference a ADO.NET driver library without adding dependencies to the main project.

The class library should, at minimum, target `netstandard20` and `net60`.

## Define nuget metadata in the `.csproj`

This step is optional. We'll do that once the PR is in if you haven't.

## Implementation

DB engine adaptations are done by implementing the interface [ISqlDialect](https://github.com/jgauffin/Griffin.Data/blob/master/src/Griffin.Data/Dialects/ISqlDialect.cs). Read is API documentation or check the SQL Server implementation to understand what should be done.

## Documentation

In the root of your project, add a `ReadMe.md`. It should show an example of how to use your implementation and the required steps to get it working. 

## Tests

Add a test project. Copy the tests from `Griffin.Data.Mssql.Tests` and ensure they work with your library.

## Pull request

Once done, create a pull request.

## Implementation details

Here is some help on the way.

### INSERT method

The `Insert` method must fetch the value from an auto-increment column and set it in the right property in the entity.

Generators/Sequences should be called before INSERT if required.

### Query method

Should apply sorting, paging, and limit to the SQL statement from the `QueryOption` object.

However, the `option.Sql` text can contain all parts of a SQL query. Therefore, you should only apply changes when sorting/paging parts are missing in the SQL query.

### Paging

For paging, consider the first page to be a row limit. A `TOP(X)` statement works fine for that. 

Do note that the user can have added TOP or manual paging in the SQL query.

### Open connection method

Should create a new connection, open it using the supplied connection string, and return it once open.

