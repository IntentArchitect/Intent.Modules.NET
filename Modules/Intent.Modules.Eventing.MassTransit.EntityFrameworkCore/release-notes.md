### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.3

- Fixed: Publishing from Consumer when Outbox pattern is None will no longer execute within the same transaction as the DB.

### Version 3.3.1

- Update: Merged Outbox pattern into this module (separate Outbox pattern module is no more).

### Version 3.3.0

- New: Adds middleware logic to ensure that Subscribers are covered in a transaction and handling Entity Framework Core persistance concerns.