### Version 2.0.6

- Improvement: Updated module documentation to use centralized documentation site.

### Version 2.0.5

- Fixed: Missing using directive for MongoConfigurationExtensions.

### Version 2.0.4

- Improvement: Updated NuGet package versions.

### Version 2.0.3

- Fixed: App settings now correctly set on install and upgrade. 

### Version 2.0.2

- Improvement: Updated NuGet package versions.
- Fixed: Adding `Collection` stereotype will now override the Collection assigned to the Entity.

### Version 2.0.1

- Improvement: Removed document translations, transitioned to using entities.

### Version 2.0.0

- Improvement: Updated NuGet package versions.
- Improvement: Migrated from MongoFramework to MongoDB.Driver.

    Breaking Change: Requires update to `Intent.MongoDb.Repositories version 2.0.0.`

    Note: If you still wish to use MongoFramework, uninstall the `Intent.MongoDb` and `Intent.MongoDb.Repositories` modules and instead install:

    `Intent.MongoDb.MongoFramework`

    `Intent.MongoDb.MongoFramework.Repositories`

### Version 1.0.26

- Improvement: Updated NuGet package versions.
- Improvement: Removed internal shared project reference.
- Improvement: Added configuration stereotype to define which querying types the provider supports.

### Version 1.0.25

- Improvement: Updated NuGet package versions.

### Version 1.0.24

- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 1.0.23

- Improvement: Updated NuGet package versions.
- Fixed: Had a hard dependency against the module `Intent.AspNetCore`.
- Fixed: Missed introduction of `System.Linq` using directive in DbContext.

### Version 1.0.22

- Improvement: Updated NuGet package versions.
- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 1.0.21

- Improvement: Updated NuGet package versions.

### Version 1.0.20

- Improvement: Updated NuGet package versions.

### Version 1.0.19

- Improvement: Included module help topic.

### Version 1.0.18

- Improvement: Updated NuGet package versions.

### Version 1.0.17

- New Feature: Added mutli-tenancy support with Finbuckle for `Seperate Databases` option.

### Version 1.0.16

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.15

- Improvement: Updated NuGet packages to latest stables.

### Version 1.0.14

- Improvement: Updated NuGet package `MongoFramework` to latest(0.29.0).
- Improvement: Now support documents with Identity which is not auto-assigned.

### Version 1.0.13

- Improvement: Updated Interoperable dependency versions.

### Version 1.0.12

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 1.0.6

- Improvement: Domain Events being published now accepts cancellation token and code slightly reformatted.
- Improvement: Removed `MongoDbUnitOfWorkBehaviour` as saving of changes has been moved to the `UnitOfWorkBehaviour` template in version 4.2.2 of the `Intent.Application.MediatR.Behaviours` module.
- Fixed: An error would occur during Software Factory execution when a derived Class in the domain designer had an aggregational association to another class.

### Version 1.0.4

- Fixed: Addressed an issue where in certain scenarios nullable id's resulted in uncompilable code.

### Version 1.0.3

- Improvement: Added Document DB Provider support, allowing this module to be used in conjunction with other Document DB technologies within the same application.

### Version 1.0.1

- Fixed: Uncompilable code such as the following would be generated for Document DB entities when the primary key attribute was an `int` or a `long`:

  ```csharp
  private int? _countryId;
  public int CountryId
  {
      get => _countryId;
      set => _countryId = value;
  }
  ```

  The getter will now be generated as:

  ```csharp
  get => _countryId ?? throw new NullReferenceException("_countryId has not been set");
  ```

### Version 1.0.0

- New Feature: MongoDB database provider added using the MongoFramework library.
- New Feature: Index modeling for MongoDB.
