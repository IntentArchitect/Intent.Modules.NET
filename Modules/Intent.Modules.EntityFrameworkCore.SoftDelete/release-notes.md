### Version 1.0.3

- Improvement: Updated module NuGet packages infrastructure.
- Fixed: SoftDelete not adding filter on inheritance (especially abstract) classes.

### Version 1.0.2

- Fixed: Integrating with DbContext's SaveChanges now will execute after the possible DomainEvents dispatching.

### Version 1.0.1

- Update: Removed `ChangeTracker.DetectChanges();` as this is only necessary when `AutoDetectChangesEnabled` is disabled.

### Version 1.0.0

- New: Implements the Soft Delete pattern introduced by Justin James.
