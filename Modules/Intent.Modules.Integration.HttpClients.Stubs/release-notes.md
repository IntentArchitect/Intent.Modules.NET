### Version 1.0.0

- New Feature: Initial release. Generates developer-owned HTTP client stubs for Integration HttpClient service contracts into a dedicated `<App>.Infrastructure.Stubs` project, with an `AddStubHttpClients` registration that replaces the real clients when the `UseStub` setting is enabled.
