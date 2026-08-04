# Intent.Dapper

This module provides patterns for working with Dapper as a persistence mechanism.

## What is Dapper?

Dapper is a lightweight, high-performance micro-ORM (Object-Relational Mapper) for .NET, designed to simplify data access and manipulation in databases. Developed by Stack Overflow, Dapper allows developers to execute SQL queries and map the results to strongly-typed objects with minimal overhead. It operates by extending IDbConnection and leverages raw SQL, which makes it both fast and flexible. Dapper's primary strength lies in its simplicity and efficiency.

For more information on Dapper, check out their [official docs](https://www.learndapper.com/).

## Overview

This module generates code to work with Dapper's [`Dapper` NuGet package](https://www.nuget.org/packages/Dapper), in particular:

- Repositories, for persistence.
- Stored procedure invocations on those repositories.

## Stored procedures

A `Repository` in the Domain designer can invoke a stored procedure in any of the three ways provided by the `Intent.Modules.Modelers.Domain.StoredProcedures` module:

1. A **`Stored Procedure` element** added directly to the `Repository` (right-click the repository and choose _Add Stored Procedure_). A repository method is generated per procedure, with a parameter per procedure parameter.
2. An **`Operation` with the `[Stored Procedure]` stereotype**. The operation's own parameters are the procedure's arguments, in the order they are modelled.
3. An **`Operation` with a Stored Procedure Invocation association** onto a `Stored Procedure` element. The _Map Invocation_ mapping supplies each procedure parameter from the operation, and the _Map Result_ mapping projects the procedure's result onto the operation's return type.

The generated body executes the procedure with raw `EXEC` SQL. The Dapper API is picked from the return type:

| Return type                                 | Generated call                                                                          |
| ------------------------------------------- | --------------------------------------------------------------------------------------- |
| none                                        | `connection.ExecuteAsync(...)`                                                          |
| a `Type-Definition` (scalar)                | `connection.ExecuteScalarAsync<T>(...)`                                                 |
| a collection of an entity / `Data Contract` | `connection.QueryAsync<T>(...).ToList()`                                                |
| a single entity / `Data Contract`           | `connection.QuerySingleAsync<T>(...)` (or `QuerySingleOrDefaultAsync<T>` when nullable) |

Every generated call is passed a Dapper `CommandDefinition` so that the repository method's `cancellationToken` is honoured. Stored-procedure-backed operations are always generated as `async`, regardless of whether the operation's name ends with `Async`.

### Limitations

- **Output parameters are not supported.** A parameter whose `[Stored Procedure Parameter]` _Direction_ is `Out` or `Both` fails the Software Factory run. On the `Stored Procedure` element (option 1 above) output-ness is described by a stereotype owned by the Entity Framework Core Repositories module, which this module does not read, so those cannot be detected — they are simply generated as input parameters.
- **User-defined table type parameters are not supported.** A parameter typed as a `Data Contract` fails the Software Factory run.
- **No name-in-schema override.** The `Stored Procedure` element's own name is used as the procedure name. For option 2, the `[Stored Procedure]` stereotype's _Name_ is used, falling back to the operation's name.
- **No type / size / precision overrides.** These are described by Entity Framework Core Repositories stereotypes which this module does not read.
- **SQL Server only.** Repositories connect with `Microsoft.Data.SqlClient.SqlConnection`.
