### Version 1.1.7

- Fixed: The importer would fail silently if you didn't have a particular older version of the .NET Runtime installed.

### Version 1.1.6

- Fixed: Primary Keys with Identity properties are now correctly imported due to a recent Stereotype update.

### Version 1.1.5

- Fixed: Elements are prefixed with "db" if their name results in invalid C# code.
- Fixed: Now handles the use case of multiple elements being created with the same name

### Version 1.1.4

- Fixed: Importing tables with the same name under different schemas would result in an error.

### Version 1.1.3

- Improvement: Option to now automatically import dependant tables from the list of tables supplied using the `include_dependant_tables` configuration
- Improvement: Better schema validation for the filter file
- Improvement: `Intent.EntityFrameworkCore.Repositories` reference only added when required

### Version 1.1.2

- Improvement: Additional in-application support topics are added.

### Version 1.1.1

- Improvement: `Triggers` are now modelled in the designer as part of the import process.
- Improvement: Added support for `globally excluded` table and view columns in the filter file.
- Improvement: Added schema validation for the filter file.
- Fixed: Invalid `Attribute` names will no longer be generated.
- Fixed: `Class` member names will no longer be generated with the same name as the class.

### Version 1.1.0

- Improved: Enhanced filter functionality using JSON configuration, allowing more granular control over table/column import selection.

### Version 1.0.9

- Fixed: Importer will no longer replace Domain Entity Attributes that are of `Enum` type when the underlying SQL type is `int`, `smallint` or `bit` since developers may want to represent those fields as Enums in code.

### Version 1.0.8

- Improvement: Included module help topic.

### Version 1.0.7

- Fixed: A "Cannot cast System.DBNull to int?" exception would occur when processing certain schemas.

### Version 1.0.6

- Improvement: Import Stored Procedures from your SQL Server by name directly on a given Repository in your Domain Designer.

### Version 1.0.5

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.4

- Fixed: An issue around exceptions not propagating correctly in some scenarios.

### Version 1.0.3

- Fixed: It was not possible to connect to Azure SQL instances.

### Version 1.0.2

- Improvement: Import a subset of Tables / Views through specifying a filter file.
- Fixed: Ability to import composite Foreign Keys.

### Version 1.0.1

- Improvement: Improved support for Index importing.
- Improvement: Importer now doesn't add explicit `Index`s on foreign keys, so the model is more how you would have modeled it. The Importer detects relationships which have been remodeled from `Aggregation`(white diamond) to `Composition`(black diamond) and does not recreate the `Aggregation Relationship`.
- Improvement: Importer now doesn't add explicit `Index`s on foreign keys, so the model is more how you would have modeled it. The Importer detects relationships which have been remodeled from `Aggregation`(white diamond) to `Composition`(black diamond) and does not recreate the `Aggregation Relationship`.

### Version 1.0.0

- New Feature: Module release.
