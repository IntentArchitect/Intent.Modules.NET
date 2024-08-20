### Version 5.2.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 5.2.2

- Improvement: Updated NuGet packages to latest stables.

### Version 5.2.1

- Fixed: Missing using directive for Outbox pattern in EF DbContext.

### Version 5.2.0

- Improvement: Updates based on changes made in `Intent.Eventing.MassTransit`.

### Version 5.1.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 5.0.1

- Logic for saving changes to Entity Framework's unit of work has been moved to version 5.2.4 of the `Intent.Eventing.MassTransit` module which now also handles saving changes for other technologies.

### Version 5.0.0

- Updated based on changes made in `Intent.Eventing.MassTransit`.
- Added support for MySql DB for Outbox pattern.

### Version 4.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.2

- Updated supported client version to [3.4.0-pre.0, 5.0.0).

### Version 4.0.1

- Update: Updates due to underlying code changes.
- Update: Updated Module version of .NET to 6.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.3

- Fixed: Publishing from Consumer when Outbox pattern is None will no longer execute within the same transaction as the DB.

### Version 3.3.1

- Update: Merged Outbox pattern into this module (separate Outbox pattern module is no more).

### Version 3.3.0

- New: Adds middleware logic to ensure that Subscribers are covered in a transaction and handling Entity Framework Core persistance concerns.
