using System.Data.SqlClient;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Queries;
using Griffin.Data.SqlServer;

namespace Griffin.Data.Tests;

public class ScaffolderTest
{
    [Fact]
    public async Task Generate_mappings()
    {
        var dialect = new SqlServerDialect();
        var s = new MapperScaffolder();
        var connectionString = "Data Source=.;Initial Catalog=Sample;Integrated Security=True";
        await s.Generate(dialect, connectionString, @"C:\temp\DemoProject");
    }

    [Fact]
    public async Task Generate_queries()
    {
        var s = new QueryScaffolder();
        var connectionString = "Data Source=.;Initial Catalog=Sample;Integrated Security=True";
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await s.Generate(connection, @"C:\temp\DemoProject");
    }
}
