using System.Reflection;
using System.Security.Cryptography;
using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Dialects;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;
using TestApp.Entities;

var mappingRegistry = new MappingRegistry();
mappingRegistry.Scan(Assembly.GetExecutingAssembly());

var config = new DbConfiguration("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True")
{
    MappingRegistry = mappingRegistry,
    Dialect = new SqlServerDialect()
};
var session = new Session(config);

IDiff diff = new Diff();
var compareService = new CompareService(mappingRegistry, diff);

var current = new MainTable
{
    Name = "Arne",
    Age = 14,
    Children = new[]
    {
        new ChildTable(ActionType.Simple, new SimpleAction(){Simple = 3, Id = 1}){Id = 1},
        new ChildTable(ActionType.Extra,new ExtraAction(){Extra = "Yo", Id=2}){Id = 2}
    },
    Money = 5L,
    Rocks = true
};
current.AddLog("bbbb");

var snapShot = new MainTable
{
    Name = "Arne",
    Age = 13,
    Children = new[]
    {
        new ChildTable(ActionType.Extra,new ExtraAction(){Extra = "Yo!", Id = 2}){Id = 2}
    },
    Money = 5L,
    Rocks = true
};
snapShot.AddLog("aaaa");

compareService.Compare(snapShot, current, mappingRegistry.Get<MainTable>(), 1);

var entity = await session.GetById<MainTable>(8);
Console.WriteLine(entity);
Console.ReadLine();
/*


await session.Insert(e2);
session.SaveChanges();
*/