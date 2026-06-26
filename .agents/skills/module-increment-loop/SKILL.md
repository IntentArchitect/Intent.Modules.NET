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
- `module-building-strategies` — for every design decision (decomposition, template vs factory extension, managed modes, verification strategy)
- `file-builder-expert` — for CSharpFile fluent API work
- `intent-metadata-consumer` — for reading stereotypes / model properties
- `intent-module-orchestrator` — for ContainerRegistrationRequest, factory extensions, cross-template wiring
- `intent-mapping-architect` — for entity↔DTO mapping templates
- `intent-domain-interactions-expert` — for handler/processing-handler interaction patterns

## Governing Principle — Industry Standard First

Every implementation decision must be grounded in the technology's documented patterns first. When Intent's patterns add constraints, use those as the second layer. Improvise only where both are silent — and log every improvisation in `PATTERN-DOCUMENT.md`'s Decision Log.

**Before starting any increment:** read `[ModuleFolder]/PATTERN-DOCUMENT.md` and `[ModuleFolder]/ATTACK-PLAN.md`.
- Check the Decision Log — do not re-derive or re-open any closed decision.
- Check Open Questions — if one is now answerable, close it before proceeding.
- Check the Progress Tracker — do not re-implement any increment marked ✅ Complete.

**After completing each increment:** update the Progress Tracker row for that increment to ✅ Complete (or ❌ Blocked with the blocker noted). Add any new decisions to the Decision Log. Close any Open Questions resolved during this increment.

## Musts

1. **Follow the full iteration cycle for every change, no exceptions.** Skipping or reordering produces output that drifts from the designer.
2. **Build the module before deploying its DLL.** A stale or broken module DLL silently runs against the previous template logic.
3. **Reload the module into the target sample after every module rebuild.** IA caches the installed DLL in memory at solution-open time — see "Dev-Loop Friction" below for the reload workaround.
4. **Run SF on the module's own designer first (if the change touched the module designer)**, then on the target sample.
5. **Inspect the SF-generated output before applying staged changes.** Read the diff and confirm it matches the intent of the template change.
6. **For eventing/messaging modules, exercise the publish + subscribe path against a running sample before declaring an increment done.** Compilation alone is not sufficient.
7. **Read the diagnostics file (if the tech emits one) before assuming the runtime is wired correctly.** NServiceBus writes `[AppBin]/.diagnostics/[EndpointName]-configuration.txt` — check the `Manifest-MessageTypes[*].IsEvent` flag, `Receiving.MessageHandlers` list, and `AssemblyScanning.Assemblies` before debugging the template.
8. **Capture every non-obvious learning to memory or to the relevant implementation skill** before closing the increment. A learning lost between increments compounds: by increment 5 the next agent walks into a wall the first agent already documented.
9. **Reset the regeneration baseline before trusting any staged diff.** The reference app is the Phase-1 SF target, and its files were hand-crafted. Before relying on a staged diff (or a "0 changes" result): delete-to-regenerate every 100%-Intent-managed template-owned file, and strip any hand-added `Ignore`/merge-protected stub on a template-owned region. Otherwise SF reproduces the hand-craft and the diff proves nothing. See `module-building-strategies` §4.
10. **Verify in two phases — reproduce, then from scratch.** Phase 1: confirm the module reproduces the reference app's existing code exactly. Phase 2 (mandatory): build a brand-new app, install the module with no code pre-written, and confirm everything generates correctly and is fully wired. Phase 2 is the only test that exposes a forgotten wiring step. Attempt app creation via the `create_application`/`create_solution` MCP tools; if they're unavailable, ask the user to scaffold the from-scratch app.

## Must Nots

1. **Never edit a generated file directly to validate a template change.** This bypasses the designer and produces drift between what the template emits and what's on disk. The only valid path is template change → SF → apply → inspect. The Spike sample is *not* an exception.
2. **Never run SF on the target sample before rebuilding and redeploying the module DLL.** The target SF uses the in-memory module assembly; stale DLL → stale output → wasted cycle.
3. **Never declare an increment done on green compilation alone.** Compilation proves syntax. Behaviour requires a run.
4. **Never silently work around a friction point.** If the loop hits a tool gap (e.g. DLL lock during reinstall), capture it in memory and surface it. These gaps are exactly what this skill is here to eliminate over time.
5. **Never batch multiple increments into a single SF cycle "to save time".** One increment, one cycle, one verification. Batched failures are exponentially harder to isolate.
6. **Never add NuGet packages by editing `NugetPackages.cs` directly.** That file is `[DefaultIntentManaged(Mode.Fully)]` — the next SF run will silently regenerate it and drop your change. All NuGet declarations must go through the Module Builder designer via MCP. See `intent-module-builder` Learnings for the full story.
7. **Never skip steps 5–7 because the user said "commit" or "move on".** A user instruction to commit does not close the cycle. Commit the module-side code only, state that steps 4–7 remain open, record the open cycle in `/WORKING.md`, and close it before touching any template file again.
8. **Never treat a transient SF failure as a dead end.** If `run_software_factory` exits with an error before reaching staging, retry once immediately. Only escalate to the user if the second attempt also fails.
9. **Never call `install_or_update_modules` when the module version has not changed.** When the module is already installed at the correct version, IA hot-reloads the recompiled DLL automatically — no reinstall is needed. Calling `install_or_update_modules` unnecessarily causes file-lock contention (`Exceeded maximum retries to save module`) and can trigger catastrophic staged-change cascades where SF proposes mass deletion of all template outputs. Valid reasons to reinstall: (a) the module is not yet installed in the target application, or (b) the imodspec version has been bumped.
10. **Never investigate or apply staged changes when SF proposes mass deletions or reports `hasErrors: true` with an empty `errors` list.** This pattern indicates IA session state corruption — typically from repeated rapid install→SF sequences. The staged changes are artifacts of corrupted state, not real proposals. Close Intent Architect completely, reopen the solution in a fresh instance, and re-run SF before taking any further action.

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
   └─ If SF exits with an error before staging: retry once immediately.
      Only escalate to the user if the second attempt also fails.
      Never accept manual file edits as a substitute for a failed SF run.
5. Read the staged diff for the affected file(s)      → confirm shape
   └─ For [IntentMerge] files: diff the entire method body against the
      prior committed state. Any line dropped that was there before must
      be intentional — the merge may be silently removing user content.
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

## Dev-Loop: Module DLL Reload

**The correct workflow — no DLL copying required:**

Intent Architect monitors the module's build output folder. When you recompile the module `.csproj`, IA detects the change and automatically picks up the new assembly the next time SF runs. There is **no need to copy DLL files manually** and **no need to close IA**.

```
1. Edit the template body
2. dotnet build <Modules/Intent.Modules.X/X.csproj> --no-incremental   ← IA picks this up automatically
3. run_software_factory(target_app_id)                                   ← uses the freshly built DLL
```

**NEVER copy DLL files manually.** The OS may lock them, you may overwrite the wrong version, and it bypasses IA's module management entirely.

**Detect a stale run:** If `run_software_factory` returns `0 changes` after a template body change that should have produced output, confirm the `dotnet build` succeeded (exit 0) before assuming the template is wrong.

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

**Pre-increment version check (run before starting each increment):**
- [ ] The module's imodspec/csproj version matches the agreed release version for this task. If the version was bumped in a prior increment, do not bump it again. Version numbers reflect intent — never change the version number to resolve a build or SF failure.

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
- [ ] Progress Tracker in `ATTACK-PLAN.md` updated (✅ Complete or ❌ Blocked with reason)
- [ ] Any new decisions added to the Decision Log in `PATTERN-DOCUMENT.md`
- [ ] Any resolved Open Questions closed in `PATTERN-DOCUMENT.md`
- [ ] Every template or factory extension implemented in this increment calls `AddNugetDependency(...)` in its constructor for each framework type it references in generated output. This applies to factory extensions too — an extension that enriches another module's template with library-dependent code must declare the NuGet dependency, since the consuming project will not have the library installed.
- [ ] Every factory extension that emits a `services.AddXxx(configuration)` call has a corresponding `AppSettingRegistrationRequest` published in `OnBeforeTemplateExecution` to seed the required configuration section in `appsettings.json`. Without this, `configuration.GetSection(...)` returns null at startup with no helpful error.
- [ ] The reference app exercises more than the minimum path: at least one scenario covers an edge case or multi-instance variant (e.g. two handlers, multiple subscriptions, a cross-service scenario). A module verified only against a single basic case will silently fail in real applications.

**On the final increment only — documentation health gate (required before declaring the loop done):**
- [ ] `<authors>` is not `Intent.Modules.NET`
- [ ] `<summary>` and `<description>` are not `A custom module for Intent.Modules.NET`
- [ ] `<tags>` is not empty
- [ ] `release-notes.md` version heading matches the `.imodspec` `<version>` and contains at least one bullet point

**On the final increment only — interoperability gate (modules with companion or bridging modules):**
- [ ] If this module auto-installs companion or bridging modules when specific platform or feature modules are already present (identified at kickoff U10 or recorded in `CONTEXT.md`/`WORKING.md`), the `.imodspec` contains an `<interoperability>` block declaring which installed platform/feature module triggers auto-installation of each bridging module. Use the MediatR module's `<interoperability>` section as the reference pattern.

If any box can't be ticked, the increment is not done — keep iterating.

---

## What "Done" Looks Like (Loop Exit Criteria)

This loop terminates when **all** of these are true:

1. Every increment in the Attack Plan has passed its per-increment checklist
2. The module's full surface (every template, every factory extension) has been exercised at least once against a real running sample
3. No `NotImplementedException` / `TODO` / placeholder bodies remain in any generated file
4. SF on the target app produces zero staged changes (the generated output and the model are in sync) — verified *after* the regeneration baseline was reset (Must #9), so the result is real and not a hand-craft echo
5. **From-scratch verification passed (Must #10):** a brand-new app with no pre-written code, with the module installed, generates correct and fully-wired output that builds and runs
6. A clean shutdown of the sample doesn't surface any errors or warnings related to the module
7. A `CONTEXT.md` file has been created or updated in the module folder, consolidating all durable technical decisions, architecture constraints, and findings from `PATTERN-DOCUMENT.md` and `ATTACK-PLAN.md`.
8. The temporary files `PATTERN-DOCUMENT.md` and `ATTACK-PLAN.md` are deleted from the module folder.
9. The project-wide `/WORKING.md` file is deleted or cleared if the entire project/task is now complete.
10. **Documentation health gate** — every `.imodspec` in the module folder passes all of the following (check each field literally, reject any that still contain the scaffold default):
   - `<authors>` is **not** `Intent.Modules.NET`
   - `<summary>` is **not** `A custom module for Intent.Modules.NET`
   - `<description>` is **not** `A custom module for Intent.Modules.NET`
   - `<tags>` is **not** empty (`<tags />` or `<tags></tags>`)
   - `release-notes.md` exists, the first heading version matches the `<version>` in the `.imodspec`, and it has at least one bullet point of content

When all these hold, hand back to the calling agent / user with:
- The list of increments completed
- The path to the module `.csproj` and the target sample
- The updated `CONTEXT.md` path


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
