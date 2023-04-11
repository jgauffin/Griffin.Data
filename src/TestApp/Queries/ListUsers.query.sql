declare @name varchar(40) = 'hej';

select *
from maintable
where name=@name;

