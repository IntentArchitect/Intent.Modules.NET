# Intent.Modules.MongoDb — CONTEXT

## Architectural decisions

### `AddMongoCollection<T>` lifetime — Scoped, not Singleton (since v2.0.10-pre.1)

`MongoConfigurationExtensionsTemplatePartial.cs` (`Templates/MongoConfigurationExtensions/`) registers
`IMongoCollection<T>` via:

```csharp
services.AddScoped(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<T>(mongoConfiguration.CollectionName);
});
```

This MUST remain `AddScoped`, not `AddSingleton`. Do not "optimize" this to Singleton — it is
intentional and load-bearing for multi-tenant (separate-database) applications:

- In separate-database multi-tenant apps (e.g. `Intent.Modules.MongoDb`'s "Separate Database" data
  isolation setting, paired with `Intent.AspNetCore.MultiTenancy`), `IMongoDatabase` itself is resolved
  **per-tenant** and registered as **Scoped** — the connection factory picks the tenant's database based
  on the current `ITenantInfo`/`IMultiTenantContextAccessor`, which is only available within a request
  scope.
- If `IMongoCollection<T>` were Singleton, the DI container would either throw
  `InvalidOperationException: Cannot resolve scoped service 'IMongoDatabase' from root provider` (if built
  strictly), or — worse — silently capture whichever tenant's `IMongoDatabase` was resolved on the FIRST
  request and use it for every subsequent request/tenant. That is silent cross-tenant data corruption,
  not just a startup crash.
- This was originally discovered as a runtime bug during the Finbuckle.MultiTenant 6.13.1 -> 9.4.10
  upgrade (see `.module-builder/Intent.Modules.MongoDb/WORKING.md` history / git log around the
  `feature/finbuckle-upgrade` branch), reproduced in `Tests/MongoDb.MultiTenancy.SeperateDb`, hand-patched
  in the generated app first, then fixed durably in the template so future SF runs (and brand-new
  separate-database MongoDb apps) don't regress back to Singleton.

### Interaction with RoslynWeaver merge

The `MongoConfigurationExtensions` class carries `[IntentManaged(Mode.Fully, Body = Mode.Merge)]` at the
class level (inherited from the assembly-level `[assembly: DefaultIntentManaged(Mode.Fully)]` plus the
template's own class attribute). In `Body = Mode.Merge`, RoslynWeaver merges statements **additively** —
it will not remove a statement that exists in the file but not in the template output, and it will not
overwrite an existing statement that already matches structurally. Practically: if an app already has a
hand-patched `AddScoped` registration (as `Tests/MongoDb.MultiTenancy.SeperateDb` did before this fix),
running Software Factory against the corrected template produces **zero staged changes** — the merge
recognizes the existing statement already matches and leaves it (and any hand-added explanatory comments)
untouched. The protective value of the template fix is for **new** separate-database MongoDb apps (or any
regenerated file where the method body doesn't already exist) — those will now get `AddScoped` from
scratch instead of the old `AddSingleton`.

## Module/app interactions

- Depends on `Intent.AspNetCore.MultiTenancy` for the per-tenant `IMongoDatabase` resolution this
  registration relies on (see `interoperability` / `detect` entries in `Intent.MongoDb.imodspec`).
- Verified end-to-end against `Tests/MongoDb.MultiTenancy.SeperateDb` (Finbuckle separate-database
  scenario) using the `finbuckle-mongo` Docker container (MongoDB 7, localhost:27017, no auth) — tenant1
  and tenant2 customers land in `MongoDbMultiTenancySeperateDb-tenant1` / `-tenant2` respectively, and
  cross-tenant fetch returns 404.
