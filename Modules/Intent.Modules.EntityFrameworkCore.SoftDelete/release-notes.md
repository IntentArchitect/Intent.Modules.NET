### Version 1.0.6

- Improvement: Moved Soft Delete pattern to `Intent.Entities.SoftDelete` (which is installed when you update to this module version). It is safe to remove this module.

### Version 1.0.5

- Fixed: SoftDelete being added to child class and parent class EF Configurations when parent class wasn't `abstract`.

### Version 1.0.4

- Fixed: Considers `Is Deleted` as set by infrastructure so it cannot be accidentally set by mapping systems.

### Version 1.0.3

- Improvement: Updated module NuGet packages infrastructure.
- Fixed: SoftDelete not adding filter on inheritance (especially abstract) classes.

### Version 1.0.2

- Fixed: Integrating with DbContext's SaveChanges now will execute after the possible DomainEvents dispatching.

### Version 1.0.1

- Update: Removed `ChangeTracker.DetectChanges();` as this is only necessary when `AutoDetectChangesEnabled` is disabled.

### Version 1.0.0

- New: Implements the Soft Delete pattern introduced by Justin James.
