### Version 4.0.4

- Fixed: Properties on composite entities were not being ignored when they should be (for example for `DomainEvents`).
- Fixed: DbSet names bug fixed which seemed to have resurfaced.
- Update: Fill Factors can now be configured from Indexes thanks to the new property on the Index element from the RDBMS module.
- Update: `EntityTypeConfiguration` files will output now to their respective relative folders if Domain Entities are organized into folders in the designer. This will alleviate any potential file output conflicts.

### Version 4.0.3

- Fixed: No longer showing two or more Partition Keys when opening up the Domain designer after updating to the latest EF Core Module version.

### Version 4.0.2

- Update: Pattern for Partition Key now defaults to Primary Key by default.
- Fixed: Partition Key management on Domain Designer bugs.
- New: Added support for .NET 7 and EF 7.

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
 * Fixed: Migration and DB creating issues with a ClassA 1-->* ClassB 1-->1 ClassC.

### Version 3.3.13
 
 * Fixed: Compositional One-to-Many relationships modelled as "HasMany" instead of "OwnsMany".

### Version 3.3.12
 
 * New: Support for associated Value Objects

### Version 3.3.11

 * New: Domain designer can now recognize Partition Keys when Cosmos DB is set as the Database Provider.
 * Update: Code in DbContext and Entity Type Configuration has been cleaned up when generating code.