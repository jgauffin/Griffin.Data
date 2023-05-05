using System.Reflection;
using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.SqlServer;
using TestApp.Entities;

var mappingRegistry = new MappingRegistry();
mappingRegistry.Scan(Assembly.GetExecutingAssembly());

var connectionString = "Data Source=.;Initial Catalog=GriffinData;Integrated Security=True";

var config = new DbConfiguration(connectionString)
{
    MappingRegistry = mappingRegistry, Dialect = new SqlServerDialect()
};
var session = new Session(config, Array.Empty<IChangeTracker>());

var result = await session.Query<MainTable>()
    .Where(new { FirstName = "Jonas" })
    .DoNotLoadChildren()
    .Paging(2, 10)
    .OrderByDescending(x => x.Age)
    .List();

var current = new MainTable
{
    Name = "Arne",
    Age = 14,
    Children = new[]
    {
        new ChildTable(ActionType.Simple, new SimpleAction { Simple = 3, Id = 1 }) { Id = 1 },
        new ChildTable(ActionType.Extra, new ExtraAction { Extra = "Yo", Id = 2 }) { Id = 2 }
    },
    Money = 5L,
    Rocks = true
};
current.AddLog("bbbb");

var snapShot = new MainTable
{
    Name = "Arne",
    Age = 13,
    Children = new[] { new ChildTable(ActionType.Extra, new ExtraAction { Extra = "Yo!", Id = 2 }) { Id = 2 } },
    Money = 5L,
    Rocks = true
};
snapShot.AddLog("aaaa");

//compareService.Compare(snapShot, current);

var entity = await session.GetById<MainTable>(8);
Console.WriteLine(entity);
Console.ReadLine();
/*


await session.Insert(e2);
session.SaveChanges();
*/
