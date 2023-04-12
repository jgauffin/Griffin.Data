using System.Data.SqlClient;
using Griffin.Data.Scaffolding.Mapper;

namespace Griffin.Data.Tests;

public class ScaffolderTest
{
    [Fact]
    public async Task Test1()
    {
        var s = new MapperScaffolder();
        await using var connection = new SqlConnection("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True");
        connection.Open();
        await s.Generate(connection, @"C:\temp");
    }
}
