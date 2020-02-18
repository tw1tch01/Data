# Data

Simple Data enhancement library based on the Repository and Specification pattern.

## Setup

This library is implemented as .NET Standard 2.1 class library.

Ensure that you have the [latest version](https://dotnet.microsoft.com/download/dotnet-core) of ASP.NET Core installed, as the unit and integration test
projects are .NET Core 3.1 applications.

###### NuGet
Setup a local NuGet source on your PC/Workstation. To do so, perform the following steps:

In Visual Studios 2019,
1. Open `Tools > NuGet Package Manager > Package Manager Settings`
2. From the side menu, select `NuGet Package Manager > Package Sources`
3. Click add (the big green `+` symbol, top right)
4. Enter a name for the source (preferably your computer name)
5. For the source, point to a local folder (e.g. `D:\NuGet`)

## Commands

To use this library in any of your applications, you need to build and publish a nuget package file.
Once you are ready to export the source, run the following command,

> dotnet pack -c Release -o [your local nuget source folder]

This command will build a nuget package built in Release configuration and output it directly to your local nuget source.For example:
> dotnet pack -c Release -o D:\NuGet

## Guidelines

Below are small guidelines as to how to use the library and some of its implementations.

### Dependency Injection

The `DependencyInjection.cs` class provides an easy way to wire up any dependencies necessary for this library.

### ContextScope

The `ContextScope.cs` class has a dictionary of `EntityState` and `Action<EntityEntry>`. The idea behind this class is to inject a small action
that should be run just before changes are saved to the database.

For example, you can use this action to set auditing information. To make implementation for this easier, the `PropertyExtension.cs` class includes 
some methods that will help with setting such information.

### PagedCollection

The `PagedCollection.cs` class is a basic model that represents some paging information along with the collection of items.

### IAuditedContext

`IAuditedContext.cs` is an abstraction for Microsoft's `DbContext.cs`. It exposes a few properties that are used to perform extra operations.

### Specifications

Specifications are the backbone behind this entire library. They can be used to specify simple or complex rules that need to be met in order for an object
(or entity) to match. 

`Specification.cs` class can be used to write rules for objects retrieved/in memory. Some operations (`&`, `|`, `!`) have been extended to allow for 
easy-of-use, in case you wish to perform operational logic on sets.

`LinqSpecification.cs` class is intended to be used for database queries, which uses the `AsExpression` to evaluate whether an entity matches the
specification.

**NOTE**: The `AsExpression` methods need to make use of primitive data types and properties that are directly on the entity. You cannot use methods 
or extension methods as this is an intended limitation of `Microsoft.EntityFrameworkCore`. Thus, using the different operators (`&`, `|`, `!`) 
provides great ability to keep simple critera logic in separate specifications and build up complicated queries (where they may be necessary) via the 
operators instead of in `Linq`.