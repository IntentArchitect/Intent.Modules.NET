# Intent.EntityFrameworkCore.DesignTimeDbContextFactory

## What is a DesignTimeDbContextFactory?

> You can also tell the tools how to create your DbContext by implementing the `Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<TContext>` interface: If a class implementing this interface is found in either the same project as the derived DbContext or in the application's startup project, the tools bypass the other ways of creating the DbContext and use the design-time factory instead.

[Source: learn.microsoft.com](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory)

## Why would this be necessary?

When executing migration scripts with Entity Framework Core, you typically would specify a Startup Project location to make use of the appsettings.json and DbContext configuration as part of the migration command.

In some cases this approach is not possible since you may be needing to run migration scripts in a non-`ASP.NET Core` application or you're making use of multi-tenancy which makes the default configuration of a DbContext unfeasible.

## How can this be used?

Once the module is installed, locate the `DESIGN_TIME_MIGRATION_README.txt` in the Infrastructure project, under the `Persistence` folder. This readme file will give you commands that you can run in Visual Studio or in your command terminal that will execute locate your `DesignTimeDbContextFactory` instance (also located in the same folder as the readme file) to create a `DbContext` instance as opposed to the one that's produced by your Startup project.

There is a accompanying `appsettings.json` file in the Infrastructure project too where you can supply connection strings that will be used by the `DesignTimeDbContextFactory`. The readme file will explain how to make use of that.

Example:

```powershell
dotnet ef migrations add MyMigrationChangeName --project "MyApplication.Infrastructure" -- {ConnectionStringName}
```

You can omit the `ConnectionStringName` which will default to using the "DefaultConnection" connection string name.
