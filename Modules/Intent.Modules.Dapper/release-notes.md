### Version 1.0.0-beta.13

- Feature: Repository operations and `Stored Procedure` elements which are backed by a stored procedure now generate an `EXEC`-based Dapper invocation instead of a `NotImplementedException` stub. All three ways of modelling this are supported: a `Stored Procedure` element on the repository, an `Operation` with the `[Stored Procedure]` stereotype, and an `Operation` with a Stored Procedure Invocation association (using its _Map Invocation_ and _Map Result_ mappings). See the module's README for the supported return shapes and the known limitations (no output parameters, no user-defined table types, SQL Server only).

### Version 1.0.0

- Improvement: Replaced obsolete System.Data.SqlClient with Microsoft.Data.SqlClient.
- Improvement: Updated NuGet package versions.
- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated NuGet packages to latest stables. Initial release.
