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
  shape, the metadata keys used to correlate generated members back to the designer model, its
  extension points, and traps already hit by modules built against it. The first (and so far only)
  resource file covers ASP.NET Core Controllers (`Intent.AspNetCore.Controllers`).

## Module Settings

None. The module has no configurable settings — installing it is enough to make the skill available.

## Adding Coverage For Another Target Module

This module's resource-file set is expected to grow as more .NET modules are documented for AI
integration. Each new target module gets its own `File Template` element (type `Single File`) under
`Skills/`, using the `Templating Method=Markdown File Builder` file setting and
`Template Settings(Source=Lookup Type, Role=AI.Context.Skills)`, following the same flat layout as the
existing `IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_Agents` template. `SKILL.md`'s
own template is then updated to add the new resource file to its index.
