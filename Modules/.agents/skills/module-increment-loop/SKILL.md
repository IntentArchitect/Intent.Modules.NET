---
name: module-increment-loop
description: "Use after intent-module-builder produces a compiled module skeleton. Drives the iterative loop of implementing template bodies one increment at a time: change template → SF on module → DLL deploy → SF on target app → inspect → build → run → verify behaviour. TRIGGER: when a module's template stubs exist, are compiling, and one or more bodies need to be implemented and validated through running code. Terminal skill in the module-building chain — loops until the Attack Plan's increments are all verified."
argument-hint: "[Attack Plan increment number] [path to module .csproj] [target sample app .csproj]"
---

# Module Increment Loop

## Purpose

Implement one Attack Plan increment at a time, then verify it through the **full** designer → SF → install → SF → inspect → build → run cycle. This is the only skill in the chain that operates on a *running* target application; everything before it produces files.

Each pass through this loop closes one increment. Repeat the loop until the Attack Plan's success criteria are all met.

## When to load this skill

`intent-module-builder` hands off here once:
- The module's `.csproj` compiles cleanly (exit code 0)
- All template stubs and `*TemplateRegistration.cs` files exist
- The Attack Plan's increments are mapped to specific template bodies to implement

The chain that leads here:
```
module-kickoff → tech-pattern-researcher → module-ecosystem-analyst → intent-module-builder → module-increment-loop
                                                                                                    ▲   │
                                                                                                    └───┘  (loops per increment)
```

Inside this loop, load **implementation-tier skills** as needed:
- `file-builder-expert` — for CSharpFile fluent API work
- `intent-metadata-consumer` — for reading stereotypes / model properties
- `intent-module-orchestrator` — for ContainerRegistrationRequest, factory extensions, cross-template wiring
- `intent-mapping-architect` — for entity↔DTO mapping templates
- `intent-domain-interactions-expert` — for handler/processing-handler interaction patterns

## Musts

1. **Follow the full iteration cycle for every change, no exceptions.** Skipping or reordering produces output that drifts from the designer.
2. **Build the module before deploying its DLL.** A stale or broken module DLL silently runs against the previous template logic.
3. **Reload the module into the target sample after every module rebuild.** IA caches the installed DLL in memory at solution-open time — see "Dev-Loop Friction" below for the reload workaround.
4. **Run SF on the module's own designer first (if the change touched the module designer)**, then on the target sample.
5. **Inspect the SF-generated output before applying staged changes.** Read the diff and confirm it matches the intent of the template change.
6. **For eventing/messaging modules, exercise the publish + subscribe path against a running sample before declaring an increment done.** Compilation alone is not sufficient.
7. **Read the diagnostics file (if the tech emits one) before assuming the runtime is wired correctly.** NServiceBus writes `[AppBin]/.diagnostics/[EndpointName]-configuration.txt` — check the `Manifest-MessageTypes[*].IsEvent` flag, `Receiving.MessageHandlers` list, and `AssemblyScanning.Assemblies` before debugging the template.
8. **Capture every non-obvious learning to memory or to the relevant implementation skill** before closing the increment. A learning lost between increments compounds: by increment 5 the next agent walks into a wall the first agent already documented.

## Must Nots

1. **Never edit a generated file directly to validate a template change.** This bypasses the designer and produces drift between what the template emits and what's on disk. The only valid path is template change → SF → apply → inspect. The Spike sample is *not* an exception.
2. **Never run SF on the target sample before rebuilding and redeploying the module DLL.** The target SF uses the in-memory module assembly; stale DLL → stale output → wasted cycle.
3. **Never declare an increment done on green compilation alone.** Compilation proves syntax. Behaviour requires a run.
4. **Never silently work around a friction point.** If the loop hits a tool gap (e.g. DLL lock during reinstall), capture it in memory and surface it. These gaps are exactly what this skill is here to eliminate over time.
5. **Never batch multiple increments into a single SF cycle "to save time".** One increment, one cycle, one verification. Batched failures are exponentially harder to isolate.

---

## The Iteration Cycle (canonical sequence)

```
1. Implement / edit the template body (CSharpFile fluent API)
   └─ Use file-builder-expert for the API
   └─ Use intent-metadata-consumer to read stereotypes
   └─ Use intent-module-orchestrator for cross-module wiring

2. dotnet build <module.csproj>                      → must exit 0

3. Reload module into the target sample's .intent/modules/
   └─ See "Dev-Loop Friction" below — the live IA process locks the DLL

4. run_software_factory(target_app_id)                → review staged change count
5. Read the staged diff for the affected file(s)      → confirm shape
6. apply_staged_file_changes(target_app_id)
7. dotnet build <target_sample.sln>                   → must exit 0
8. Run the target sample
9. Exercise the increment's behaviour (HTTP request, message publish, etc.)
10. Verify expected log lines / handler invocations / diagnostics
11. Stop the sample
12. Mark the increment done OR identify the next fix and return to step 1
```

The cycle is fractal — within a single increment, a failed verification at step 9 may require returning to step 1 with a smaller change. That's fine. What matters is that every change goes through the full cycle.

---

## Dev-Loop Friction: Module DLL Reload

**The problem:** Intent Architect loads installed modules from `<target-intent>/.intent/modules/<ModuleId>.<Version>/lib/<Module>.dll` into memory at solution-open time. While IA is running, the OS holds an exclusive lock on the DLL — you cannot overwrite it. `install_or_update_modules` MCP refreshes from IA's *cached* source (NuGet/feed), not from a local source rebuild. `run_software_factory` uses the in-memory DLL → no template changes picked up.

**Current workaround (until IA MCP exposes a reload tool):**

1. Build the module: `dotnet build <Modules/Intent.Modules.X/X.csproj> --no-incremental`
2. Ask the developer to close *just the IA window for the target sample* — no need to touch other open solutions
3. Copy the freshly-built DLL + PDB over the installed location:
   ```
   cp <Modules/.../bin/Debug/net8.0/X.dll>  <target/intent/.intent/modules/X.<Ver>/lib/X.dll>
   cp <Modules/.../bin/Debug/net8.0/X.pdb>  <target/intent/.intent/modules/X.<Ver>/lib/X.pdb>
   ```
4. Reopen the target sample via `open_solution`
5. Run SF — picks up the rebuilt template

**Detect the symptom:** If `run_software_factory` returns `0 changes` after a template body change that should have produced output, the DLL is stale. Don't debug the template — fix the dev-loop first.

**Long-term:** This friction is exactly the kind of thing a future MCP `reload_modules` tool should solve. Capture it whenever you hit it.

---

## Verification by Module Type

Different module categories need different verification at step 9. The minimum criterion: *something observable in a real runtime confirms the behaviour the template intended*.

### Eventing / Messaging (pub/sub)
- HTTP request publishes an event → corresponding `IIntegrationEventHandler<T>.HandleAsync` log line appears
- HTTP request that maps to a routed integration command → command handler is invoked at the configured endpoint
- For transports with diagnostic files (e.g. NServiceBus): check `Manifest-MessageTypes[*].IsEvent`, `Receiving.MessageHandlers`, `Recoverability.ErrorQueue`

### Persistence / EF Core
- Domain operation → row in the target table (or in-memory equivalent)
- Migration applied / schema generated as expected

### API / Controllers / Endpoints
- HTTP request against the generated route → expected status code and shape
- Swagger spec contains the operation

### DI / Configuration
- The relevant `Add<X>(IServiceCollection, IConfiguration)` lands in the right startup file at the right priority
- The resolved service can be requested from the container at runtime

### Authentication / Authorization
- Unauthenticated request → 401; valid token → 200; insufficient scope → 403

---

## Per-Increment Checklist

Before moving to the next increment in the Attack Plan:

- [ ] Template body implemented using the right implementation-tier skill (file-builder-expert / orchestrator / metadata-consumer / mapping-architect / domain-interactions-expert)
- [ ] Module `.csproj` builds with exit code 0
- [ ] Module DLL deployed into the target sample (dev-loop workaround applied if needed)
- [ ] Target sample SF run shows the expected staged changes — and *only* the expected staged changes (unexpected drift is a red flag)
- [ ] Staged changes inspected and applied
- [ ] Target sample `.sln` builds with exit code 0
- [ ] Target sample runs without startup errors
- [ ] Increment's behaviour exercised against the running sample
- [ ] Observable result confirms behaviour (log line / response / diagnostic entry)
- [ ] Sample stopped cleanly
- [ ] Any non-obvious finding captured to memory or to the relevant skill

If any box can't be ticked, the increment is not done — keep iterating.

---

## What "Done" Looks Like (Loop Exit Criteria)

This loop terminates when **all** of these are true:

1. Every increment in the Attack Plan has passed its per-increment checklist
2. The module's full surface (every template, every factory extension) has been exercised at least once against a real running sample
3. No `NotImplementedException` / `TODO` / placeholder bodies remain in any generated file
4. SF on the target app produces zero staged changes (the generated output and the model are in sync)
5. A clean shutdown of the sample doesn't surface any errors or warnings related to the module

When all five hold, hand back to the calling agent / user with:
- The list of increments completed
- The path to the module `.csproj` and the target sample
- Any captured learnings worth promoting into the artifact backlog (memory: `project-nservicebus-learnings-for-artifacts.md` is the precedent)

---

## Capturing Learnings Mid-Loop

Inside the loop, you will hit small surprises: an SDK quirk, an undocumented obsolete method, a runtime gotcha. Capture each one immediately — *not* at the end. Routes:

| Type of finding | Where it goes |
|---|---|
| CSharpFile builder gotcha (obsolete API, edge case) | `file-builder-expert/SKILL.md` Musts/Must Nots or `resources/troubleshooting.md` |
| Stereotype / model-reading pattern | `intent-metadata-consumer/SKILL.md` |
| Cross-module wiring pattern | `intent-module-orchestrator/SKILL.md` |
| Tech-specific runtime gotcha (e.g. NServiceBus convention) | `project-<tech>-learnings-for-artifacts.md` memory + the relevant skill if it generalises |
| Dev-loop friction (tool gaps, IA limitations) | `project-module-dev-loop-gap.md`-style memory, plus a note in this skill |

If you're unsure where it goes, default to the closest existing skill and let a future pass refine.

---

## Source of Truth

- `intent-module-builder/SKILL.md` — handoff target
- `project-module-dev-loop-gap.md` (memory) — current friction inventory
- `project-nservicebus-learnings-for-artifacts.md` (memory) — example of the per-tech learnings file shape
- `feedback-intent-module-workflow.md` (memory) — the canonical workflow rule this skill enforces
