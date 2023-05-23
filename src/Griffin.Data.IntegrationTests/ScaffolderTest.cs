using System.Data.SqlClient;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Queries;
using Griffin.Data.SqlServer;

namespace Griffin.Data.IntegrationTests;

public class ScaffolderTest
{
    private const string Directory = @"D:\src\jgauffin\Griffin.Data\demo\DemoApp";
    private const string ConnectionString = @"Data Source=.;Initial Catalog=Sample;Integrated Security=True";

    [Fact]
    public async Task Generate_mappings()
    {
        var dialect = new SqlServerDialect();
        var s = new MapperScaffolder();

        //var dir = @"C:\temp\DemoProject";
        await s.Generate(dialect, ConnectionString, Directory);
    }

    [Fact]
    public async Task Generate_queries()
    {
        var s = new QueryScaffolder();
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await s.Generate(connection, @Directory);
    }
}
