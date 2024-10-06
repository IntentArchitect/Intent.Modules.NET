### Version 1.0.10

- Fixed: AutoFixture code around setting up test data wasn't handling `DateOnly` fields correctly.

### Version 1.0.9

- Fixed: When testing services with a Domain Entity called `Client` it will no longer generate conflicting code.

### Version 1.0.8

- Fixed: Updating this module force version update of IntegrationTesting module.

### Version 1.0.7

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.6

- Improvement: Updated NuGet package infrastructure.

### Version 1.0.5

- Fixed: Bug where SF would crash around CRUD services without `Delete`, `Update` and /or `GetAll` implementations.

### Version 1.0.4

- Improvement: Generate `NotImplemenetedException` stub tests for, CRUD style services which we can't determine implementations for.

### Version 1.0.3

- Fixed: Crash around services don't have an Update operation.

### Version 1.0.2

- Fixed: Documentation link fixed.

### Version 1.0.1

- Improvement: Updated trait configuration and setup to allow for multiple types of trait configurations.

### Version 1.0.0

- New Feature: Asp.Net Core Integration Testing CRUD module.
