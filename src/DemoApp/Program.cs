// See https://aka.ms/new-console-template for more information


using System.IO;
using System.Reflection;
using DemoApp.Data.Accounts;
using DemoApp.Data.Todolists;
using DemoApp.Domain.Accounts;
using DemoApp.Domain.Todolists;
using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;
using Griffin.Data.SqlServer;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json", false)
    .Build();


var configurationString = config.GetConnectionString("Db")!;

var dbConfig = new DbConfiguration(configurationString)
    .AddMappingAssembly(Assembly.GetExecutingAssembly())
    .UseSnapshotChangeTracking()
    .UseSqlServer();

using var session = new Session(dbConfig);

var accountRepos = new AccountRepository(session);
var user = new Account("MyName", "SomePassword", "SomeSalt");
await accountRepos.Create(user);

var repos = new TodolistRepository(session);
var entity = new Todolist("My list", user.Id)
{

}
repos.Create()
