using Griffin.Data.Scaffolding.Mapper;

namespace Griffin.Data.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var s = new MapperScaffolder();
        await s.GenerateMappings("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True", @"C:\temp");
    }
}
