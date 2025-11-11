### Version 1.0.9

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.8

- Fixed: Incorrect `ICurrentUser` method being used for CosmosDB implementation
- Improvement: Updated `SetAuditableFields` to be `SetAuditableFieldsAsync`

### Version 1.0.7

- Fixed: Updated template to use the updated `ICurrentUserService` implementation.

### Version 1.0.6

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 1.0.5

- Improvement: Included module help topic.

### Version 1.0.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.3

- Improvement: Updated Designer script to not modify attributes if they already exist.
- Improvement: Added support for different UserId Types (guid, long, int).
> ⚠️ **NOTE**
> Standardized to `UserIdentifer` for auditing logic which can work with `UserId` or `UserName`.
- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 1.0.2

- Fixed: Any manually added attributes called `CreatedBy`, `CreatedDate`, `UpdatedBy` or `UpdatedDate` would be automatically removed on creation, regardless of whether or not auditing was ever applied to an entity.
- Improvement: The DbContext now ensures that its impossible to update the `CreatedBy` and `CreatedDate` column values through use of `entry.Property("<name>").IsModified = false` statements.

### Version 1.0.1

- Fixed: Small variable correction.

### Version 1.0.0

- New Feature: Initial release.
