### Version 4.0.4

- Update: Changed from overriding `int SaveChangesAsync(CancellationToken)` to instead override `int SaveChangesAsync(bool, CancellationToken)` on the DbContext as the latter is called by the former in the base type.
- Update: `DispatchEvents`'s name has changed to have an `Async` suffix and now uses a `CancellationToken`.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Change: Moved `DomainEvents` on `Domain Entities` decoration moved to `Intent.DomainEvents` module.


### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.10

- Fixed: Placement of DomainEvents property is now only targeting Classes that are Aggregate Roots and not inheriting from Aggregate Roots.