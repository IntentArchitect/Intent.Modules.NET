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
