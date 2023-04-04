create table MainTable
(
	Id int not null identity primary key,
	Name varchar(40) not null,
	Age smallint not null,
	Money bigint not null,
	Rocks bit null,
);

create table ChildTable
(
	Id int not null identity primary key,
	MainId int not null,
	ActionType int not null
);

create table ExtraAction
(
	Id int not null identity primary key,
	ChildId int not null constraint FK_ExtrAction_ChildTable foreign key references ChildTable(Id),
	Extra varchar(40) not null
);

create table SimpleAction
(
	Id int not null identity primary key,
	ChildId int not null constraint FK_SimpleAction_ChildTable foreign key references ChildTable(Id),
	Simple int
);

create table Logs
(
	MainId int not null constraint FK_Logs_MainTable foreign key references MainTable(Id),
	CreatedAtUtc datetime not null,
	Message varchar(2500) not null
);

create table SharedMain
(
	Id int not null identity primary key,
	SomeField int not null
);

create table SharedChild
(
	Id int not null identity primary key,
	MainId int not null constraint FK_SharedChild_SharedMain foreign key references SharedMain(Id),
	ParentProperty varchar(20) not null,
	Value varchar(40)
);

delete from Logs;
delete from ExtraAction;
delete from SimpleAction;
delete from MainTable;
delete from ChildTable;

DBCC CHECKIDENT ('[MainTable]', RESEED, 0);
DBCC CHECKIDENT ('[ExtraAction]', RESEED, 0);
DBCC CHECKIDENT ('[SimpleAction]', RESEED, 0);
DBCC CHECKIDENT ('[ChildTable]', RESEED, 0);

insert into MainTable (Name, Age, Money, Rocks) VALUES('Mine', 38, 39093289238, 1);
declare @id int;
select @id = scope_identity();
insert into Logs (MainId, Message, CreatedAtUtc) VALUES(@id, 'Hejsan på er', GETUTCDATE());
insert into ChildTable (MainId, ActionType) VALUES(@id, 1);
declare @childId1 int;
select @childId1= scope_identity();
insert into ChildTable (MainId, ActionType) VALUES(@id, 2);
declare @childId2 int;
select @childId2 = scope_identity();
insert into SimpleAction (ChildId, Simple) VALUES(@childId1, 3);
insert into ExtraAction (ChildId, Extra) VALUES(@childId2, 'dlkjdfkdl');

insert into SharedMain (SomeField) VALUES(113);
select @id = scope_identity();
insert into SharedChild (MainId, ParentProperty, Value) VALUES (@id, 'Left', 'Stop');
insert into SharedChild (MainId, ParentProperty, Value) VALUES (@id, 'Right', 'Skip');
