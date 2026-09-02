# Context: Intent.Blazor.HttpClients

## Purpose

Generates typed `HttpClient` proxies for Blazor applications against referenced RESTful service contracts, plus the `HttpClientConfiguration` static class that registers them (`AddHttpClients`) and resolves each proxy's base address from `Urls:{ApplicationName}` configuration.

## Invariant: this module depends on no authentication module

`Intent.Blazor.HttpClients` depends on neither `Intent.Blazor` nor `Intent.Blazor.Authentication`, and **must not acquire either dependency**. A Blazor application with service proxies and hand-rolled or no authentication is a supported configuration, as is a non-Blazor consumer (`CleanArchitecture.Comprehensive`, `MinimalHostingModel` install this alongside `Intent.Blazor.WebAssembly`, with no auth module at all).

Everything below follows from that constraint.

## Decision: the authorization handler is host-supplied, but the WebAssembly one stays the default (2026-08-30)

`HttpClientConfiguration` is generated **once**, into the `.Client` project, and in a two-project Blazor application **both hosts call `AddHttpClients`** — the server reaches it through `AddClientServices`. So whatever handler `AddHttpClients` attaches lands in the _server's_ DI container too. The handler it attaches, `AuthorizationMessageHandler`, is a WebAssembly component: it caches the token and the built `Authorization` header in unsynchronised instance fields, which is fine for a single-user browser and actively unsafe on a server, where one handler instance is shared across every request and every user for the handler chain's lifetime.

The fix is **not** to change what `AddHttpClients` attaches. It is to let each host supply its own:

- `AddHttpClients` **keeps** attaching `AuthorizationMessageHandler` by default. It is the fallback for every consumer that has no host supplying a handler, and with no auth module installed the generated output is byte-identical to before this change. Do not make this conditional on an auth module — this module cannot see one.
- A second method, **`AddApiAuthorizationHandler(IServiceCollection, IConfiguration, Func<IServiceProvider, string[], DelegatingHandler>)`**, is emitted whenever any proxy `RequiresAuthorization`. It references only BCL types, adds no NuGet dependency, and has no default caller — uncalled it is simply an inert public method. Re-opening the typed client with `AddHttpClient<TClient, TImplementation>()` is **additive**: it appends to the same named client's `HttpMessageHandlerBuilderActions`. The duplicate typed-client registration this creates is identical to the first and harmless. Do **not** try to reconstruct the framework's internal client name string to avoid it.

## Cross-module contract: two metadata keys

A host-side module removes the default handler and wires its own. That handshake is **metadata, not text matching** — the previous implementation in `Intent.Blazor.Authentication` matched on the emitted string `"AddHttpMessageHandler"`, which would break the moment anything else added a handler to the same chain.

| Key                                                                | Set on                                                                   | Meaning                                                                                       |
| ------------------------------------------------------------------ | ------------------------------------------------------------------------ | --------------------------------------------------------------------------------------------- |
| `authorization-handler` (`AuthorizationHandlerMetadataKey`)        | each default `AddHttpMessageHandler` chain statement in `AddHttpClients` | "this statement is the default handler — a host that supplies its own removes it by this key" |
| `api-authorization-handler` (`ApiAuthorizationHandlerMetadataKey`) | the template's `CSharpFile`                                              | "`AddApiAuthorizationHandler` is emitted in this file"                                        |

**Why the second key exists, and why it is set in the constructor.** A consumer needs to know whether `AddApiAuthorizationHandler` was emitted _before_ deciding to call it. It cannot find out by inspecting the generated class: `CSharpFile.AddClass(name, configure)` does **not** run its `configure` callback until build time, so `file.Classes[0].Methods` is empty for the whole registration phase. (Verified against `Intent.Modules.Common.CSharp` — the class shell exists, its members do not.) Setting file-level metadata eagerly in the template constructor gives an order-independent signal that any factory extension can test at any point after template registration.

Both keys are part of this module's public contract. Renaming either needs a synchronised version bump on this module and on every module that reads them.

**Graceful degradation is by design.** An older `Intent.Blazor.HttpClients` sets neither key, so a newer consumer finds nothing, skips silently, and the application keeps its previous behaviour rather than emitting a call to a method that was never generated. This is why there is no version floor declared anywhere — a hard `<dependency>` would force this module to be installed into applications that have authentication but no service proxies.

## Module Interactions

- **`Intent.Blazor.Authentication`** — the only current consumer of the contract above. It looks this template up by the string template id `Intent.Blazor.HttpClients.HttpClientConfiguration`; there is no project reference in either direction and there must not be one. It removes the `authorization-handler` statements and calls `AddApiAuthorizationHandler` once per host: the server passes a per-request `ServerAuthorizationMessageHandler`, the WebAssembly client passes the same `AuthorizationMessageHandler.ConfigureHandler(authorizedUrls:)` chain `AddHttpClients` used to build, so browser behaviour is unchanged.

## Verifying a change here

`dotnet build` proves nothing about generated output. The consumers that matter are the ones with **no** authentication module — `CleanArchitecture.Comprehensive` and `MinimalHostingModel` (authorized proxies, no auth module) must produce a **byte-identical** `HttpClientConfiguration.cs` with the inline `AuthorizationMessageHandler` intact.

> ⚠️ The MCP `patch_file` auto-formatter re-indents the contents of verbatim (`@"…"`) string literals anywhere in a file it touches, including ones far from the edit. In this template those literals **are** the generated output, so a stray reformat silently changes every consumer's generated file. Check `git diff` for whitespace-only changes inside string literals after any edit here.
