### Version 4.0.1

- Fixed: `Column` stereotype values were being ignored for `Primary Key` attributes.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.
- Update: DbMigrationsReadMe content updated to match EF Core commands in Visual Studio and CLI.

### Version 3.3.16

* Fixed: Primary key issues in deep compositional trees.
* Fixed: DbSet names sometimes reflected the namespace of the Entity it was referring to. Its now normalized to only take the Entity name.

### Version 3.3.15

 * Fixed: Cosmos DB Partition Key logic in Domain Designer for Inherited Classes.
 * Fixed: Migration and DB creationg issues with a ClassA 1-->* ClassB 1-->1 ClassC.

### Version 3.3.13
 
 * Fixed: Compositional One-to-Many relationships modelled as "HasMany" instead of "OwnsMany".

### Version 3.3.12
 
 * New: Support for associated Value Objects

### Version 3.3.11

 * New: Domain designer can now recognize Partition Keys when Cosmos DB is set as the Database Provider.
 * Update: Code in DbContext and Entity Type Configuration has been cleaned up when generating code.