create table MainTable
(
	Id int not null identity primary key,
	Name varchar(40) not null,
	Age smallint not null,
	Money bigint not null,
	Rocks bit null,
	ActionType int not null
);

create table ExtraAction
(
	Id int not null identity primary key,
	MainId int not null constraint FK_ExtrAction_MainTable foreign key references MainTable(Id),
	Extra varchar(40) not null
);

create table SimpleAction
(
	Id int not null identity primary key,
	MainId int not null constraint FK_SimpleAction_MainTable foreign key references MainTable(Id),
	Simple int
);

create table Logs
(
	MainId int not null constraint FK_Logs_MainTable foreign key references MainTable(Id),
	CreatedAtUtc datetime not null,
	Message varchar(2500) not null
);


delete from Logs;
delete from ExtraAction;
delete from SimpleAction;
delete from MainTable;

DBCC CHECKIDENT ('[MainTable]', RESEED, 0);
DBCC CHECKIDENT ('[ExtraAction]', RESEED, 0);
DBCC CHECKIDENT ('[SimpleAction]', RESEED, 0);

insert into MainTable (Name, Age, Money, Rocks, ActionType) VALUES('Mine', 38, 39093289238, 1, 2);
declare @id int;
select @id = scope_identity();
insert into Logs (MainId, Message, CreatedAtUtc) VALUES(@id, 'Hejsan på er', GETUTCDATE());
insert into ExtraAction (MainId, Extra) VALUES(@id, 'dlkjdfkdl');
insert into SimpleAction (MainId, Simple) VALUES(@id, 3);
