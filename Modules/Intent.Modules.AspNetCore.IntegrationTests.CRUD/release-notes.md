### Version 1.0.15

- Improvement: Updated Tests to support Composite CRUD implementations.
- Fixed: Bug where `Domain Operation` tests where not generating.

### Version 1.0.14

- Improvement: Add support for generating test implementations for `Domain Operation` based CRUD implementations.
- Fixed: Updated dependencies based on the changes made in the `Intent.AspNetCore.IntegrationTesting` module.

### Version 1.0.13

- Fixed: Fixed an issue where Test Implementations for Composite children was not generating the correct code.
- Improvement: Included module help topic.

### Version 1.0.12

- Improvement: Updated module icon

### Version 1.0.11

- Improvement: Improvements in generated http client code: recommended implementation of `IDisposable pattern`, `JsonSerializerOptions` only added if required, as well as small synatax updates
- Improvement: Default values set on non-nullable properties on `HttpClientRequestException`
- Improvement: Updated tests to use `.Any()` instead of `.Count() > 0`
- Improvement: MediaType `constant` defined and reused in http client, instead of duplicated string literals

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
