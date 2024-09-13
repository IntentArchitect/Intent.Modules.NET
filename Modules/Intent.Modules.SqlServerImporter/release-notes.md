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
