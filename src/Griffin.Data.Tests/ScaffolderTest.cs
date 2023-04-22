using Griffin.Data.Scaffolding.Mapper;

namespace Griffin.Data.Tests;

public class ScaffolderTest
{
    [Fact]
    public async Task Test1()
    {
        var s = new MapperScaffolder();
        var constr = "Data Source=.;Initial Catalog=Sample;Integrated Security=True";
        await s.Generate("SqlServer", constr, @"C:\temp\DemoProject");
    }
}
