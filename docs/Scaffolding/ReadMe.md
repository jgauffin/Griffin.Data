Scaffolding
============

Scaffolding generates classes, mappings, and tests.

Start by installing the scaffolder from a command prompt:

```
dotnet tool install -g griffin.data.scaffolding
```

Next, go into your data project folder and run it.

```
dotnet gd generate
```

Once done, you will have th queries, mappings, entities and everything required.

Reorganize the files to your liking.

# Requirements

The scaffolder assumes that you are using `XUnit` in your test projects and `FluentAssertions` as the assertion libraries. IF you don't either make an exception for the `Data.Tests` project or simply delete the generates tests.

You can also add a [GitHub issue](https://github.com/jgauffin/Griffin.Data/issues/new?label=Scaffolding) where you request support for other test libraries.

# Conventions

The scaffolder uses some conventions to identify which project to generate code in.

## Naming conventions

### Namespaces

The scaffolder uses project names to find the root namespace. A project file named `MyApp.Domain.csproj` will get files generated with `MyApp.Domain` as root namespace.

### Hierachies

To create hierachies in the class structure, use the root table name in the child tables.  The hirearchies are used to be able to create folders for each group of tables.

For instance:

* `Users`
* `UserPermissions`
* `TodoTasks` 
* `TodoTaskGithubIssue`
* `TodoTaskDocumentReview`

Will generate the following namespaces (and classes):

* Users
  * User
  * Permission
* TodoTasks
  * TodoTask
  * GithubIssue
  * DocumentReview

## App project (business layer)

The project name must adhere to one the following rules:

Either contain any of:

* Application
* Business
* Core
* Domain

or end with `App`.

## Data project

The project name must adhere to one the following rules:

Either contain any of:

* Db
* Repositories
* Datalayer

or end with `Data`.
