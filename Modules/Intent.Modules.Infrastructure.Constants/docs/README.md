# Intent.Infrastructure.Constants

Generates a static `Constants` class for infrastructure values that need to be reused across generated code.

The current implementation is focused on Entity Framework connection string names. Instead of leaving connection string names as raw string literals inside generated Infrastructure registration code, this module generates constants for them and updates the generated registrations to reference those constants.

## What This Module Generates

A static `Constants` class similar to:

```csharp
public static class Constants
{
    public const string DefaultConnection = "DefaultConnection";
    public const string ReportingConnection = "ReportingConnection";
}
```

It also updates Infrastructure dependency injection code from this:

```csharp
options.UseSqlServer(
    configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
```

to this:

```csharp
options.UseSqlServer(
    configuration.GetConnectionString(Constants.DefaultConnection),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
```

## How It Works

This module listens for `InfrastructureRegisteredEvent` events published during Software Factory execution.

For supported Entity Framework infrastructure components, it reads the published connection string name from the event payload, converts it to PascalCase, and adds it as a constant field on the generated `Constants` class.

The same template then updates the generated Infrastructure dependency injection method so that matching connection string usages are rewritten to use `Constants.<Name>`.

This means the module does not discover connection strings by scanning generated source text for names. It relies on structured infrastructure event metadata published by the infrastructure-owning module.

## Supported Infrastructure

The current implementation supports the following infrastructure components:

- `SqlServer`
- `PostgreSql`
- `MySql`
- `Oracle`
- `CosmosDb`

Each infrastructure publisher must provide a `ConnectionStringName` property on the event for the constant to be generated and applied.

Duplicate connection string names are de-duplicated automatically before generation.
