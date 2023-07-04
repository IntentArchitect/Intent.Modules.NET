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

- New: MongoDB database provider added using the MongoFramework library.
- New: Index modeling for MongoDB.