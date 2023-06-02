### Version 1.1.0

- Now has full repository support for entities modelled in the Domain Designer for `Document DB`s.

### Version 1.0.2

- Changed `Update` method name to `Upsert`.
- Changed `Get` method name to `GetAsync`.
- Changed from using `Queue` to use `ConcurrentQueue`.
- Added XML documentation comments to interface.

### Version 1.0.1

- `IStateRepository`'s `Get` method can now take in a `CancellationToken`.
