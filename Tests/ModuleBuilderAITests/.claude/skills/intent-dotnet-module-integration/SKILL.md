---
name: intent-dotnet-module-integration
description: "Look up how to integrate with a specific runtime .NET module in this repo — its dependency identity, template inventory, generated shape, metadata contract, and extension points — before writing a module that hooks into its generated output. USE ONLY WHEN a module needs to reference, enrich, or dispatch into another .NET module's generated code (e.g. ASP.NET Core Controllers). DO NOT USE FOR generic cross-module wiring mechanics (see intent-module-orchestrator) or designer/model integration (see intent-modelers-integration). REQUIRES knowing which target .NET module(s) the change touches."
template-id: Intent.ModuleBuilder.AI.DotNet.Skills.IntentDotnetModuleIntegration_SkillMd_Agents
contentHash: BCDFBB607F81B8E4D1F00214A8ABF58FCE89147EC57B03EF63815221D5BF19A1
---
# Intent Dotnet Module Integration

> [!TIP]
> **Read more if you want to know about** a specific .NET module's dependency identity, template inventory, generated shape, metadata contract, and extension points in full:
> *   [ASP.NET Core Controllers](./resources/aspnetcore-controllers.md)
> *(To conserve tokens, only read the resource file(s) for the module(s) your change actually touches.)*

## Musts

1. **This skill documents the target module's facts — not the mechanics of wiring a module together.** For callback priorities, the request-publishing vs FileBuilder-mutation split, the `"model"` metadata bridge, and DI/config registration patterns, load `intent-module-orchestrator` first; this skill's resource files assume you already know those mechanics and state only what is specific to the target module.
2. **Take the dependency on the typed route**: reference the target module's own NuGet package (never a bare role-string integration) so you get its typed model interfaces (e.g. `IControllerModel`), not an untyped `ICSharpFileBuilderTemplate` plus a raw `modelId` string.
3. **Correlate generated members by metadata, never by name.** Every reference file states the exact `TryGetMetadata` keys a target module's generated class/method/parameter carry — names are transformed, de-duplicated, and overloaded, so a name match silently breaks the moment either side changes.
4. **Read the resource file's "Traps" section before writing code** — it records failure modes already hit by a module built against that target (multi-host duplication, folder off-by-ones, obsolete constants) that are not visible from the target module's public API alone.
5. **Check a resource file's "Version compatibility" section against the target module's actually-installed version before trusting a fact flagged there as version-gated.** A resource file records what held at the version it was verified against; some facts (an extension point added, changed, or removed) only hold within that stated range.

## Must Nots

1. Never infer a target module's generated shape, role strings, or metadata keys from reading its templates cold — read its resource file here first; if none exists yet for the module you need, say so rather than guessing.
2. Never correlate a generated class/method to a designer model by name — see Must #3.
3. Never assume one target module's identity split (NuGet package id vs Intent module id vs namespace) generalizes to another — each is verified independently in its own resource file.
4. Never assume a resource file's facts hold for an installed version outside the range its "Version compatibility" section states — re-verify the specific fact against the target module's own source instead.
