### Version 1.0.0

- New Feature: On install, generates a dedicated `<App>.Infrastructure.Stubs` project (beside `<App>.Infrastructure`, inheriting its .NET settings) to hold the generated stubs.
- New Feature: Generates a `<Service>HttpClientStub` per Integration HttpClient service contract, implementing the contract with a method per endpoint that returns safe default values.
- New Feature: Generates a `StubHttpClientConfiguration` with an `AddStubHttpClients` registration that replaces the real HTTP clients with stubs when the per-group or per-service `UseStub` application setting is enabled.
