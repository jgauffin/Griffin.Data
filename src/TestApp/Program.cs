﻿using System.Data.SqlClient;
using System.Reflection;
using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Configuration;
using Griffin.Data.Dialects;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;
using Griffin.Data.Scaffolding.Queries;
using TestApp.Entities;

var mappingRegistry = new MappingRegistry();
mappingRegistry.Scan(Assembly.GetExecutingAssembly());

var connectionString = "Data Source=.;Initial Catalog=GriffinData;Integrated Security=True";

var gen = new QueryScaffolder();
var dir = Path.GetFullPath("..\\..\\..", Environment.CurrentDirectory);

var con = new SqlConnection(connectionString);
con.Open();
await gen.Generate(con, dir);

return;

var config = new DbConfiguration(connectionString)
{
    MappingRegistry = mappingRegistry, Dialect = new SqlServerDialect()
};
var session = new Session(config, Array.Empty<IChangeTracker>());

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