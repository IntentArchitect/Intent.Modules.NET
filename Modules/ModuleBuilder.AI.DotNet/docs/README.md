# Intent.ModuleBuilder.AI.DotNet

This module ships `intent-dotnet-module-integration`, an AI agent skill for building Intent Architect
modules that hook into another **runtime .NET module's** generated output. Installing it drops the
skill into the consuming repository's `.claude/skills` and `.opencode/skills` trees, alongside
whatever other Intent Architect AI skills are already installed there.

Where `Intent.ModuleBuilder.AI.Modelers` documents Intent's own built-in designers (Domain, Services,
Eventing, User Interface), this module documents the .NET modules that generate code into a consuming
application — their dependency identity, template inventory, generated shape, metadata contract, and
extension points — so an AI agent can wire a new module against, say,
`Intent.Modules.AspNetCore.Controllers` without having to read that module's templates cold.

## What Gets Installed

- **`SKILL.md`** — the skill's entry point: what it's for, its Musts/Must Nots, and an index of the
  resource files below.
- **One resource file per target .NET module** — a deep reference for that specific module, covering
  what to reference and how (NuGet package vs project reference), its template inventory and generated
  shape, how its generated code links back to the designer model (metadata keys, `RepresentedModel`,
  or the template's own model), the complete public member surface of the types a consumer calls (so
  an agent never has to reflect over or decompile the target assembly), its extension points, and
  traps already hit by modules built against it.

| Resource file | Target module | Covers |
|---|---|---|
| `aspnetcore.md` | `Intent.AspNetCore` | `Program.cs` / `Startup.cs`: adding services, middleware, endpoints and host-builder configuration |
| `aspnetcore-controllers.md` | `Intent.AspNetCore.Controllers` | enriching controller actions and dispatching from them |
| `entityframeworkcore.md` | `Intent.EntityFrameworkCore` | DbContexts, entity type configurations, `SaveChanges` hooks and `AddDbContext` registration |
| `entities-repositories-api.md` | `Intent.Entities.Repositories.Api` | implementing the repository interfaces in a persistence module, and injecting them from a handler module |

Only modules published on nuget.org are covered, so an integrating module can take a typed
`PackageReference` wherever the guide recommends one.

## Module Settings

None. The module has no configurable settings — installing it is enough to make the skill available.

## Adding Coverage For Another Target Module

This module's resource-file set is expected to grow as more .NET modules are documented for AI
integration. Each new target module gets its own `File Template` element (type `Single File`) under
`Skills/`, using the `Templating Method=Markdown File Builder` file setting and
`Template Settings(Source=Lookup Type, Role=AI.Context.Skills)`, following the same flat layout as the
existing `IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_Agents` template. `SKILL.md`'s
own template is then updated to add the new resource file to its index.
