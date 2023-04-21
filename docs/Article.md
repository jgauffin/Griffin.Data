Introduction
===========

This article is about version 2.0 of my object/relation mapper and data mapper Griffin.Data. It's a library which is designed to work in both the write and read side of your application.

Before we get into the library itself, let's talk about business entities, or domain entities as they are called in Domain Driven Design. Business entities are classes which are used to ensure that the logic is encapsulated and that changes are driven by behavior (invoking methods). 

This is a poco (Plain Old CLR Object):

```csharp
public class Account
{
    public int Id { get; set; }
    public AccountState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LockedAt { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

It has no control over it's sttate. An part of the application can modify it's state. For instance, one part can change the date fields without remember to change the state or vise versa. Finding those kind of discrepemncies is easy in the beginning but quite hard when the application is five years old.

An easy way to remendy that is to use encapsulation, one of the fundemantal principles of OOP.

```csharp
public class Account
{
    public Account(string userName)
    {
        if (userName == null ) throw new ArgumentNullException(nameof(userName));

        State = AccountState.NotVerified;
        UserName = userName;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; private set; }
    public AccountState State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public string UserName { get; private set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public void LockAccount()
    {
        State = AccountState.Locked;
        LockedAt = DateTime.Now;
    }

    public void UnlockAccount()
    {
        State = AccountState.Active;
        LockedAt = null;
    }
}
```

That change (making most setters private) forces you to add behavior to your classes and also ensures that the logic regarding accounts is in one place, no matter how old your application becomes.

If you agree with with the sentiment above, you'll probably like Griffin.Data. If you don't, EF Core or NHibernate might be better fit for you.

## Enter Griffin.Data

Many ORMs struggle with business entities. They require some sort of compromise, for instance forcing setters, default constructors or properties that isn't really part of the business domain. Those compromises violates proper encasuplation which is essential in long lived applications if you want to keep the development speed throughout the years.

Griffin.Data doesn't do that. It allows your business entities to follow all rules of encapsulation and well defined business entities.

### Getting started

The easiest way of getting started is to build an example application. In this case we are building a TODO application which can have different types of tasks.

One task might be to correct a GitHub issue while one might be to review a documentation file. A task for a github issue will contain a link to the issue while a document review task will contain a document link and a feedback field.

To make a more complete example, we've also added permissions to lists (to be able to share lists) and accounts.

Finally we would like to see what have been changed and by whom.

```sql
create table Accounts
(
    Id int identity not null primary key,
    UserName varchar(40) not null,
    Password varchar(40) not null,
    Salt varchar(40) not null
);

create table Todolists
(
    Id int identity not null primary key,
    Name varchar(40) not null,
    CreatedById int not null constraint FK_Todolists_Accounts1 foreign key references Accounts(Id),
    CreatedAtUtc datetime not null,
    UpdatedById int constraint FK_Todolists_Accounts2 foreign key references Accounts(Id),
    UpdatedAtUtc datetime
);

create table Tasks
(
    Id int identity not null primary key,
    Name varchar(40) not null,
    TaskType int not null,
    State smallint not null,
    Priority int not null,
    CreatedById int not null constraint FK_Tasks_Accounts1 foreign key references Accounts(Id),
    CreatedAtUtc datetime not null,
    UpdatedById int constraint FK_Tasks_Accounts2 foreign key references Accounts(Id),
    UpdatedAtUtc datetime
);


create table TodolistPermissions
(
    Id int identity not null primary key,
    TodolistId int not null constraint FK_TodolistPermissions_Todolists foreign key references Todolists(Id),
    AccountId int not null constraint FK_TodolistPermissions_Accounts foreign key references Accounts(Id),
    CanRead bit not null,
    CanWrite bit not null,
    IsAdmin bit not null
);



create table TaskGithubIssues
(
    TaskId int not null primary key constraint FK_TaskGithubIssues_Tasks foreign key references Tasks(Id),
    Name varchar(40) not null,
    IssueUrl int not null,
);

create table TaskDocumentReviews
(
    TaskId int not null  primary key constraint FK_TaskDocumentReviews_Tasks foreign key references Tasks(Id),
    DocumentUrl varchar(255) not null,
    Comment text
);

```

## Scaffolding

Now that we have a sound data model, let's scaffold it to get some code.

Start by installing the scaffolder (as a global tool to be able to run it in any project):

```
dotnet install -g `Griffin.Data.Scaffolder`
```

Then, in your project install the SQL Server package (support for MySQL, PostgreSQL and Oracle are being built, feel free to help.)

```
dotnet add package Griffin.Data.SqlServer
```

You now have everything required to generate the data layer. 

```
dotnet gd generate
```

The scaffolder tries to find the correct projects to put the classes in. If it does, they go into the correct place. If not, they are created in a directory structure in the same project. More information about this in the project wiki.

You should not have got the following files:

