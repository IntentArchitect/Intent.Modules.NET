### Version 4.2.2

- Added support for executing Stored Procedures. To use a Stored Procedure:
	- Create a `Repository` in the Domain Designer (either in the package root or a folder).
	- You can optionally set the "type" of the repository to a `Class` which will extend the existing repository which is already generated for it, otherwise if no "type" is specified a new Repository is generated.
	- On a repository you can create `Stored Procedure`s.
	- At this time, the module supports a Stored Procedure returning: nothing, an existing `Class` or a `Data Contract` (`Domain Object`).
	- The Software Factory will generate methods on the Repositories for calling the Stored Procedures.
- Update: Removes some warnings from generated code.


### Version 4.2.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.1.0

- Update: Repositories will only be generated for Classes created inside a "Domain Package".

### Version 4.0.2

- Update: Converted RepositoryBase into Builder Pattern version.

### Version 4.0.1

- Update: Internal template changes.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.

### Version 3.3.14

- Update: Added `IQueryable` option for `FindAsync` operation.
