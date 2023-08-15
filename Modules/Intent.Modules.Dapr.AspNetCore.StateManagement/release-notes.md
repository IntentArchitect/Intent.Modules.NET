
### Version 1.1.0

- Improvement: Added Document DB Provider support, allowing this module to be used in conjunction with other Document DB technologies within the same application.
- Improvement: Now has full repository support for entities modeled in the Domain Designer for `Document DB`s.

### Version 1.0.2

- Improvement: Changed `Update` method name to `Upsert`.
- Improvement: Changed `Get` method name to `GetAsync`.
- Improvement: Changed from using `Queue` to use `ConcurrentQueue`.
- Improvement: Added XML documentation comments to interface.

### Version 1.0.1

- Improvement: `IStateRepository`'s `Get` method can now take in a `CancellationToken`.
