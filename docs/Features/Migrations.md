Database migrations
====================

This migration library can apply migrations to your database in a forward only manner. The migrator can be placed into categories (for instance per feature) which are version managed independently from each other.

More about that later. For now, only prefix your scripts with something. For instance `Migrations_v1_Description.sql`.

As your application progresses you might have the following scripts:

* `Migrations_v1_create_tables.sql`
* `Migrations_v2_update_users.sql`
* `Migrations_v3_add_friend_feature.sql`

To apply those scripts to a database you need to provide a connection. Do note that the sequence in the middle is what decides in which order scripts should be run. The text after do not matter at all, its just for you to understand what the script contains.

You can of course zero pad the scripts to get them in order in Visual Studio:

* `Migrations_v01_create_tables.sql`
* `Migrations_v02_update_users.sql`
* `Migrations_v03_add_friend_feature.sql`

.. but that wont affect the migrator as it interprets the sequence number as a number.

Here is a code sample on how you apply the migrations:

```csharp
IDbConnection OpenConnection()
{
    var con = new SqlConnection(connectionString);
    con.Open();
    return con;
};

var migrations = new MigrationRunner(OpenConnection, "Migrations");
migrations.Run();
```

Once done, the database should have been upated up to the newest script. That is all.

## Using seperate migrations in the same application

You might have different editions of your application or extra features (plugins) that only are installed for some users. Those features might also have different release cycles.

That makes it hard to use the same migrations for those parts.

That's easy to solve. Simply use different prefixes for the files:


**Standard table**

* `Standard_v1_create_tables.sql`
* `Standard_v2_update_users.sql`
* `Standard_v3_add_friend_feature.sql`

**Feature migrations**

* `FeatureY_v1_create_feature_tables.sql`
* `FeatureY_v2_fixes.sql`

Then you just execute the migrator (you can do it from different parts of your application).

```csharp
IDbConnection OpenConnection()
{
    var con = new SqlConnection(connectionString);
    con.Open();
    return con;
};

// Standard migrations
var migrations = new MigrationRunner(OpenConnection, "Standard");
migrations.Run();


// Feature Y migrations
var migrations = new MigrationRunner(OpenConnection, "FeatureY");
migrations.Run();
```


## Using the GO statement

Each migration script is executed in its own transaction which is commited once the script completes.

You can however use `GO` on a seperate line in the scripts. That means that the script is divided into two different transactions.

```sql
create table SomeTable
(

);

GO

INSERT INTO SomeTable () VALUES();
```

However, that's typically not recommended, since if the second part fails, the schema table wont be updated with that version. That means that the migrator will try to run the same version again next time.

Instead, always try to use seperate scripts instead of GO.

## The schema table

This migrator adds a table name `DatabaseSchema` to your table.

It contains "Name" and "Version" columns. The name is the migration name ("Migrations", "Standard" and "FeatureY" in the above examples.) The versions is the latest applied version, for instance "3" for the "Standard" migration.

## Restrictions

* Forward only migrations.
* All scripts for the same migration category must be in the same table
* The migrator must be able to do both DDL and DML statements in the DB (ensure that it's run from an account that allows that).
* It's one transaction per script = Only the latest running script will be rolled back if it fails.
