# CONTEXT — Intent.Modules.AspNetCore.IntegrationTesting

## Why this module generates its own DTOs/Commands/Queries/Enums instead of referencing the Application layer

The generated `*.IntegrationTests` project does **not** reuse the real Command/Query/DTO/Enum
types from the Application project, even though it has a direct `ProjectReference` to the
generated Api project (and therefore transitively to Application/Domain) and hosts the API
in-process via `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`). Instead, the
`DtoContract`, `EnumContract`, `ProxyServiceContract`, etc. templates in this module
independently regenerate equivalent types inside the test project, then call the API through a
generated proxy.

**This is not a hard architectural requirement — it's inherited implementation reuse.**

Origin: commit `22f0ab8970` ("Wip Integration Tests module", Jan 2024) created this module by
generalizing the *existing* HTTP-client proxy-generation pipeline
(`Intent.Modules.Contracts.Clients.Shared` / `Intent.Modules.Integration.HttpClients.Shared`)
that was originally built for **genuinely external consumers** — Blazor WASM frontends, Dapr
service invocation, cross-service HTTP clients — where no assembly reference to the API is
possible. The same commit bumped release notes across `Intent.Modules.Blazor.HttpClients`,
`Intent.Modules.Dapr.AspNetCore.ServiceInvocation`, and `Intent.Modules.Integration.HttpClients`
with the identical line: *"Improvement: Underlying proxy templates updated to support alternate
Metadata models."* — i.e. the shared base was generalized to also accept CQRS
Commands/Queries/Services as its model source, and this module was plugged into it, rather than
a new "pull types straight from Application" code path being written.

**Why this still has value even though the project reference exists today:**
- Forces a genuine serialize → HTTP pipeline → deserialize round-trip using independently
  generated types, decoupling the test project's compile-time surface from internal
  Application-layer renames/restructuring that don't change the wire shape.
- Keeps the generation strategy uniform with the case where these tests *are* pointed at a
  separately deployed instance with no project reference at all (this module pulls in
  `Testcontainers.PostgreSql`, suggesting that fuller/remote test topologies are an intended
  future direction, even though the current scaffold always uses in-process `WebApplicationFactory`).

**Cost of this choice:** any defect in the shared base template propagates here. See below.

## Known shared-base defect affecting generated DTOs

`DtoContractTemplateBase` (owned by `Intent.Modules.Contracts.Clients.Shared`, not this module)
only ever emits a **parameterless** constructor that null!-initializes non-nullable reference
members:

```csharp
// Modules/Intent.Modules.Contracts.Clients.Shared/Templates/DtoContract/DtoContractTemplateBase.cs
// lines ~85-91
if (nullableMembers.Any())
{
    @class.AddConstructor(ctor =>
    {
        ctor.AddStatements(nullableMembers); // no parameters ever added
    });
}
```

A `ConstructorParameters()` helper exists in the same file (~lines 195-207) but is **never
called** from the constructor-generation path. Result: if the corresponding application-layer
DTO has a parameterized/required constructor, the test-project copy generated here only gets a
parameterless one (e.g. `BrandDto()`, `CountryDto()` in generated test output) — which is the
"invalid constructor" symptom reported against this module. The bug lives in the shared base,
not in this module's templates, so fixing it here means either patching the shared base or
adding a factory-extension-level workaround in this module — confirm which before touching
`DtoContractTemplateBase` directly, since `Intent.Modules.Contracts.Clients.Shared` is consumed
by other modules too (see below).

## Related/affected modules

- `Intent.Modules.Contracts.Clients.Shared` — owns `DtoContractTemplateBase`; this module's
  `DtoContractTemplatePartial`/`DtoContractTemplateRegistration` extend it. Any fix to the
  constructor-generation defect affects every consumer of this base, not just IntegrationTesting.
- `Intent.Modules.Integration.HttpClients.Shared` — sibling consumer of the same
  "alternate metadata model" proxy generalization introduced alongside this module.
- `Intent.Modules.Blazor.HttpClients`, `Intent.Modules.Dapr.AspNetCore.ServiceInvocation` —
  other consumers of the shared proxy/DTO pipeline; same generation lineage, same potential
  exposure to the constructor defect if they hit the same nullable-member + required-constructor
  shape.

## Generation mechanics

- `DtoContractTemplateRegistration.GetModels()` discovers DTOs via
  `_metadataManager.GetServicesAsProxyModels(application)` →
  `RegistrationHelper.GetReferencedDTOModels(p, true)`, not via the standard
  `Application.Contracts.Clients` DTO discovery path.
- Generated test project (`*.IntegrationTests.csproj`) has a real `ProjectReference` to the
  generated Api project and uses `Microsoft.AspNetCore.Mvc.Testing` for an in-process
  `TestServer` — so reusing real Application types is technically possible today; it's a design
  choice inherited from history, not a current technical constraint.
