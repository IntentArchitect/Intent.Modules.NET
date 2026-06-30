### Version 1.0.0

- New Feature: On install, generates a dedicated `<App>.Infrastructure.Stubs` project (beside `<App>.Infrastructure`, inheriting its .NET settings) to hold the generated stubs.
- New Feature: Generates a `<Service>HttpClientStub` per Integration HttpClient service contract, implementing the contract with a method per endpoint — data-returning methods are scaffolded with safe default values, while `void`/`Task` methods throw `NotImplementedException` as an implement-me placeholder. Method bodies are developer-owned (`Body = Mode.Ignore`) — customisations are preserved across Software Factory runs while the signature stays in sync with the contract.
- New Feature: Endpoints returning `PagedResult<T>` generate a single-item page that reflects the request — `TotalCount`/`PageCount` of `1`, and `PageNumber`/`PageSize` echoed from the query's paging fields — instead of an all-zero result.
- New Feature: Generates a `StubHttpClientConfiguration` with an `AddStubHttpClients` registration that replaces the real HTTP clients with stubs when the per-group or per-service `UseStub` application setting is enabled.
