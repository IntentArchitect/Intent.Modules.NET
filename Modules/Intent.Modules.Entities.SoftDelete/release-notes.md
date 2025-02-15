### Version 1.0.0

- New Feature: Migrated Soft Delete pattern from Intent.EntityFrameworkCore.SoftDelete in order to provide Soft Delete capabilities to Cosmos DB.
- Fixed: Setting an Entity's Delete flag when `private setter` is `on` will now work through a method call, very similar to Basic Auditing.