using Griffin.Data.Scaffolding;

namespace Griffin.Data.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var s = new Scaffolder2();
        await s.Do("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True", @"C:\temp");
    }
}