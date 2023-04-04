create table Accounts
(
	Id int not null identity primary key,
	AccountType int not null,
	userName varchar(20),
);

create table CompanyAccount
(
	Id int not null identity primary key,
	CompanyName varchar(200) not null,
);

create table UserAccount
(
	Id int not null identity primary key,
	FirstName varchar(200) not null,
	LastName varchar(200) not null,
);

create table Posts
(
	Id int not null identity primary key,
	Title 
);