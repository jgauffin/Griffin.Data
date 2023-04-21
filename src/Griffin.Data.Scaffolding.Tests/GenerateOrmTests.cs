using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.Scaffolding.Tests;

public class GenerateOrmTests
{
    [Fact]
    public async Task Should()
    {
        await GeneratorHelper.GenerateOrm("sqlserver", "data source=.;initial catalog=Sample;integrated security=true",
            @"C:\temp\DemoProject");
    }
}
