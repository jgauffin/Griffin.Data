
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

create table TodolistPermissions
(
    Id int identity not null primary key,
    TodolistId int not null constraint FK_TodolistPermissions_Todolists foreign key references Todolists(Id),
    AccountId int not null constraint FK_TodolistPermissions_Accounts foreign key references Accounts(Id),
    CanRead bit not null,
    CanWrite bit not null,
    IsAdmin bit not null
);


create table TodoTasks
(
    Id int identity not null primary key,
    TodolistId int not null constraint FK_TodoTasks_Todolists foreign key references Todolists(Id),
    Name varchar(40) not null,
    TaskType int not null,
    State smallint not null,
    Priority int not null,
    CreatedById int not null constraint FK_Tasks_Accounts1 foreign key references Accounts(Id),
    CreatedAtUtc datetime not null,
    UpdatedById int constraint FK_Tasks_Accounts2 foreign key references Accounts(Id),
    UpdatedAtUtc datetime
);


create table TodoTaskGithubIssues
(
    TaskId int not null primary key constraint FK_TaskGithubIssues_TodoTasks foreign key references TodoTasks(Id),
    Name varchar(40) not null,
    IssueUrl int not null,
);

create table TodoTaskDocumentReviews
(
    TaskId int not null  primary key constraint FK_TaskDocumentReviews_TodoTasks foreign key references TodoTasks(Id),
    DocumentUrl varchar(255) not null,
    Comment text
);