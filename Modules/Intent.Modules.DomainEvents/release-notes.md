### Version 5.0.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.0.3

- Improvement: Added `TODO` comments on `NotImplementedException`.

### Version 5.0.2

- Fixed: Inserting Domain Event publishing will also take into effect the State mode of an Entity class.

### Version 5.0.0

- New Feature: Updated module to cater for new Advanced Mapping feature.

### Version 4.1.2

- Improvement: Entity Domain Eventing infrastructure can now be configured to be on all aggregates or only aggregates with modeled domain events.

### Version 4.1.1

- Improvement:`Publish` method on the service interface now accepts a `CancellationToken`.

### Version 4.1.0

- Improvement: Added auto-implementation of Domain Events being published from class constructors and methods.
- Fixed: Enum namespaces not being automatically added to Domain Event classes.

### Version 4.0.4
- Added: Comments on `DomainEvent`s and their `Property`s are now added as XmlDocComments on the generated classes.
- Upgrade: `DomainEventTemplate` using CSharpFileBuilder paradigm.

### Version 4.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Change: Moved `DomainEvents` on `Domain Entities` now happening from this module.


### Version 3.3.4

- Fixed: Domain events not respecting folder paths from the designer.
