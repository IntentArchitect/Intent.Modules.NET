### Version 1.2.1

- Improvement: Will now automatically create appropriate `Role`s in the `Visual Studio` designer upon initial installation.
- Improvement: Will now publish an event to indicate that `launchsettings.json` requires an `http://...` `applicationUrl`.
### Version 1.1.0

- Fixed: Doesn't overwrite `AddControllers()` statement anymore but apply only the `AddDapr()` where needed.

### Version 1.0.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 1.0.2

- Updated supported client version to [3.2.0, 5.0.0).

### Version 1.0.1

- Relative location of Dapr configuration files updated to align with single `.sln` file paradigm.