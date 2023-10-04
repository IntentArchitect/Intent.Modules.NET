### Version 1.0.6

- Improvement: Domain Events being published now accepts cancellation token and code slightly reformatted.

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