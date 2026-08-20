### Version 1.0.0-beta.14

- Fixed: Generated `AddAsync` no longer hardcodes `entity.Id` when assigning the database-generated key back onto the entity; it now assigns the entity's actual modelled primary key property, so repositories for entities whose primary key isn't named `Id` compile correctly.
- Fixed: `AddAsync` now inserts and captures composite primary key columns per-attribute based on the `Primary Key` stereotype's Data source, instead of assuming a single-attribute key is auto-generated and excluding every other key attribute from the `INSERT`.
- Fixed: Composite-key `WHERE` clauses in `FindByIdAsync`, `RemoveAsync`, and `UpdateAsync` were missing a space between predicates, producing invalid SQL such as `OrderId = @OrderIdAND ProductId = @ProductId`.
- Fixed: Entities with no `Primary Key`-stereotyped attribute no longer generate `FindByIdAsync`, `UpdateAsync`, or `RemoveAsync` on the repository. Previously `FindByIdAsync` generated invalid C# (an empty tuple parameter type), and `UpdateAsync`/`RemoveAsync` generated SQL with an empty `WHERE` clause that would have matched every row in the table. `AddAsync` and `FindAllAsync`, which don't need a key, are unaffected.

### Version 1.0.0-beta.13

- Feature: Repository operations and `Stored Procedure` elements which are backed by a stored procedure now generate an `EXEC`-based Dapper invocation instead of a `NotImplementedException` stub.

### Version 1.0.0

- Improvement: Replaced obsolete System.Data.SqlClient with Microsoft.Data.SqlClient.
- Improvement: Updated NuGet package versions.
- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated NuGet packages to latest stables. Initial release.
