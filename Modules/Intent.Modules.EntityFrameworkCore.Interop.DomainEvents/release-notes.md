### Version 4.1.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.1.0

- Improvement: Converted Decorators into Factory Extensions.

### Version 4.0.9

- Fixed: Added more explicit typing in around `Task`.

### Version 4.0.8

- Improvement: Removed code to inject a `null` parameter value when the `Intent.EntityFrameworkCore.DesignTimeDbContextFactory` is present.
- Improvement: Domain Events being published now accepts cancellation token and code slightly reformatted.

### Version 4.0.7

- Fixed: Fix up based on change made in `Intent.EntityFrameworkCore.DesignTimeDbContextFactory`.

### Version 4.0.6

- Improvement: Necessary internal update for DbContext save changes hook-in points.

### Version 4.0.5

- New Feature: Synchronous DBContext SaveChanges support for domain event dispatching.

### Version 4.0.4

- Improvement: Changed from overriding `int SaveChangesAsync(CancellationToken)` to instead override `int SaveChangesAsync(bool, CancellationToken)` on the DbContext as the latter is called by the former in the base type.
- Improvement: `DispatchEvents`'s name has changed to have an `Async` suffix and now uses a `CancellationToken`.

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Improvement: Moved `DomainEvents` on `Domain Entities` decoration moved to `Intent.DomainEvents` module.


### Version 4.0.0

- New Feature: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.10

- Fixed: Placement of DomainEvents property is now only targeting Classes that are Aggregate Roots and not inheriting from Aggregate Roots.
