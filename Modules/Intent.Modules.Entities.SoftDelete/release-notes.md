### Version 1.2.0

- Improvement: Moved this pattern over to a `SoftDeleteInterceptor` for better separation and a cleaner implementation.

### Version 1.1.0

> ⚠️ **NOTE**
>
> It is highly recommended to update to at least this version as previous versions of this module would generate code which not did prevent owned entities from being hard deleted.

- Fixed: Depending on which other modules were installed, the `SaveChanges` overrides in `DbContext` classes would not be generated meaning that `SetSoftDeleteProperties` would not be called.
- Fixed: Owned entities / value objects of soft deleted items would be fully deleted.

### Version 1.0.1

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5.

### Version 1.0.0

- New Feature: Migrated Soft Delete pattern from Intent.EntityFrameworkCore.SoftDelete in order to provide Soft Delete capabilities to Cosmos DB.
- Fixed: Setting an Entity's Delete flag when `private setter` is `on` will now work through a method call, very similar to Basic Auditing.
- Fixed: Filter not added to child entity configuration, additionally, UI validation to give error if `Soft Delete` added to a child entity.