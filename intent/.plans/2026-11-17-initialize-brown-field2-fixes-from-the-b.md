# initialize-brown-field2: fixes from the ClientA run

## Context

You ran the new skill on a clean copy of ClientA's codebase. I compared its model with the old skill's model of the same code in an earlier copy of ClientA's codebase, element by element, read-only from both solutions' metadata. I also read the run's conductor and all nine sub-agent transcripts. The new run's speed came mostly from what it didn't model.

## Old skill vs new skill, same code (ClientA's service)

| Designer | Element | Old | New | What's missing in the new run |
| --- | --- | --- | --- | --- |
| Services | Service | 67 | 23 | **All 44 business-layer services:** `Order` (66 ops), `OrderProcess`, `OrderMatchProcess`, `UserManagementProcess`, `Notification`, `Region`, `LookupValues`… The 23 controllers match exactly |
| | Operation / Parameter | 439 / 577 | 158 / 176 | The business services' operations |
| | Perform Invocation | 330 | 44 | Controller → business service and service → service calls |
| | Query / Create / Update / Delete Entity Action | 190 / 25 / 17 / 6 | 81 / 18 / 15 / 1 | The new run hung them on controller operations instead, one intent per action |
| | Enum (literals) | 19 (60) | 0 | Every enum: `OrderTypesEnum`, `Roles`, `Notice`, `SignalRKey`, `TradeType`… |
| | DTO | 144 | 97 | 48 DTOs: the SignalR payloads, ERP DTOs (`STOCKDTO`, `P_*DTO`), `LogDTO`, `FinanceOrderContractDTO`, `ContractDefaultsDTO`, legends… |
| | Type-Definition | 9 | 0 | `Stream`, `Uri`, `dynamic`, `IActionResult`… |
| | Service Proxy | 7 | 6 | `AzureBlobStorage`, `AcmeApi` (the new run added `Erp`) |
| | Diagrams | 15 | 9 | Fewer areas, because there's no business layer to draw |
| Domain | Class | 68 | 59 | 3 logging tables and 6 unmapped classes (`contracts`, `contracts1`, `contracts2`, `contractTypes`, `seasonNumbers`, `regionSources`): 215 attributes |
| | Class Constructor | 15 | 0 | One per scaffolded class (13 classes differ by exactly 1 member) |
| | Association | 37 | 29 | Associations to the 9 left-out classes |
| Codebase Structure | Solution / C# Project / Project Reference | 1 / 6 / 7 | 0 / 0 / 0 | The whole solution mirror: projects with their `.NET Settings` and `C# Project Options`, and references |
| | File Classifications | 18 | 0 | Classification was skipped in this run |

Same DTO and controller shapes where both modelled them. The domain columns, keys and constraints largely agree.

## Why each gap happened

1. **The business layer (44 services, 286 invocations, about 120 Entity Actions).**
   - The survey doesn't look for application services, so the question round offered "24 controllers, 192 actions".
   - The conductor's brief narrowed the layer to "the HTTP surface".
   - The Services agent read a business object, then **flattened**: it followed controller → BO → `Repo.*<T>` and put the Entity Actions on the controllers.
   - `modelling-services.md` never says what an application service is, and lost the old template's Public / Internal split (`ReverseEngineerServicesSkillTemplatePartial.cs:148-150, 226`).
   - **Knock-on:** with `Erp` unmodelled as a service, the stock reader matched the `Erp` proxy to the BO class `Erp`. That misread is part of why a house module was built.
2. **Enums, 48 DTOs and 2 proxies.** The Services agent modelled only what was reachable from the HTTP surface ("90 of 147 DTOs reachable … rest left out"). The Integration agent skipped `AzureBlobStorage` and `AcmeApi`. Nothing forced completeness, because the skill keeps every opt-in rule off (`SKILL.md:139`, `drift-engine.md:7,48`, `modelling-domain.md:5`).
3. **Domain's 9 classes and 15 constructors.** The plumbing user decision (logging tables) and the agent's own call ("unmapped, nothing uses them"; "collection-initialising constructors left out"). With the opt-in rules off, nothing reported them.
4. **Codebase Structure.** The skill skips the mirror on purpose (Visual Studio `"false"`, `SKILL.md:111,126`), because I dropped the old skill's step 4.

## How the run went (from the transcripts)

- **What worked:**
  - Throwaway extractor scripts in `%TEMP%` gave exact names fast, and Services cross-checked its parse against grep (149 actions in the code against 144 parsed).
  - Each layer agent piloted a slice first.
  - Model-only probes proved a clean run had checked something, and every planted break was caught first time.
- **Where agents lost time:**
  - **Evidence.** They hunted for a log line no tool returns, and for the engine README (`strings` on the DLL, a stopped background `find`). Each rediscovered `model-links.json` and wrote its own counter.
  - **House-module setup.** Both module agents hit the same snags, and the second repeated the first's reported mistakes:
    - output at the repo root;
    - NU1605 and the SDK version;
    - `supportedClientVersions` hand-edited;
    - missing test packages;
    - `#nullable` stripped;
    - learning the engine API by reflection.
  - **Reading.** Every sub-agent `cat`ed the whole skill and all its references, whatever the brief said.
  - **Contradiction.** The planting brief contradicted the skill (shell vs Edit tool).
- **The survey was shallow.** Two sub-agents ran 1 minute each and found none of the business layer, the unmapped classes or the house HTTP wrappers.

## Changes (all in `D:\WIP\initialize-brown-field2\`)

### A. Model the whole application layer (gaps 1, 2)
- **survey.md § Layers:**
  - Services counts endpoints **and** application services separately. An application service is a class in the business or application layer that endpoints or other services call, injected or `new`ed: `*BO`, `*Process`, `*Service`, `*Manager`, use-case classes.
  - It also counts enums and DTOs per project, and every outbound client class.
  - The survey report line and the phase 2 Services option name all of them.
- **modelling-services.md § Services and operations:** port the Public / Internal split.
  - Every application-service class is a Service, its public methods are Operations, and it has no HTTP settings.
  - The endpoint gets a `Perform Invocation` to it, and the service operation carries its own Entity Actions, proxy invocations and invocations of other services. **Never flatten through it.**
  - Extract row: `AppServices | class | DI or new | public methods | what each calls`.
  - Generic example: `OrdersController.Get` → `OrderManager.GetOrders` → Query `Order`.
- **modelling-services.md § Contracts:** model every DTO and enum the code declares in the layer's projects, and Type-Definitions for library types used in signatures. Not only those reachable from endpoints.
- **modelling-integration.md:** every outbound client class is a Service Proxy candidate.
- **SKILL.md hand-off:** a brief never restates or narrows what a layer covers; scope limits come only from a quoted User decision.

### B. Opt-in rules on by default (gaps 2, 3)
- **SKILL.md phase 5:** after a layer's baseline is clean, the layer agent adds its opt-in rows to the Root Folder's `Drift Check`, each Glob narrowed to the layer's In folders. The rows:
  - Domain: `optin/unmodelled.type`, `.member`, `.index`, `.value`;
  - Services: `.type`, `.member`, `.endpoint`, `.exposed`, `.interaction`, `.value`;
  - all layers: `.file`.
- **Triage:** the agent triages every finding to empty: model it, or add an Exception row with a reason (event-log tables, a cache helper). This is what makes the gaps above impossible to leave silently.
- **User decision paragraph:** "Opt-in rules are on for every modelled layer; a rule is off, or code excepted, only with a stated reason."
- **Other files:** `drift-engine.md` § Drift Check is rewritten, with an Exception example. The "stay off" lines in `modelling-*.md` and `SKILL.md:139,205` go. Phase 9 lists every Exception.
- **The plumbing decision** becomes "left off the intent links; excepted with a reason when an opt-in rule reports it".

### C. Domain completeness (gap 3)
**modelling-domain.md:**
- Model every entity class in the entities folders, mapped or not.
- Model constructors as the code declares them.
- Leaving a class out needs an Exception with a reason.

### D. Mirror the solution in Codebase Structure (gap 4)
**Assumption** (no question tool this turn; say if wrong): .NET gets the old skill's mirror via `Intent.VisualStudio.Projects`.
- **What is ported** from `D:\WIP\ibrefactor\initialize-brown-field\SKILL.md` steps 1, 3 and 4 (lines 98–103, 120–157) into SKILL.md phase 3:
  - the solution;
  - one `C# Project (.NET)` per `.csproj` with its `.NET Settings` and `C# Project Options`;
  - Project References;
  - central package management;
  - Materialize Folder;
  - startup-file generation off.
- **Check:** an SF run whose `.sln`/`.csproj` diff is empty or harmless; a destructive diff is a model gap.
- Other stacks get Folders mirroring the source roots. Drift Scope stays on the Root Folder and Folders.

### E. Evidence on disk (agents' time)
**drift-engine.md § Reading a run** replaces the log line with:
- the report path, and how to apply it;
- a ready `node -e` over `<solution>/<app>/.intent/model-links.json` counting linked elements per designer and type, against the elements modelled;
- the model-only probe.

The same goes into the phase 5 and 9 report formats.

### F. Extraction scripts as the default for regular code (what worked)
SKILL.md and each `modelling-*.md` § Extract:
- for regular code, write a throwaway extractor in `%TEMP%` that emits a spec, and generate the designer scripts from it;
- cross-check counts with grep;
- pilot one slice first;
- use read-only sub-agents for irregular code and call-chain tracing.

### G. Deeper survey
**survey.md:** each survey agent also lists
- application services and how controllers reach them;
- entity classes no DbContext maps;
- house wrappers around `HttpClient`, the bus or a cache, with an example each.

### H. House-module setup checklist
**house-modules.md** gets:
- set `relativeLocation: ../modules/<Module>` immediately after `create_application`;
- SDK and `Intent.Modules.Common` versions, and `supportedClientVersions`, matching the installed engine's `.imodspec`;
- test packages, and run the test DLL if `dotnet test` refuses;
- `#nullable` in source files;
- a short engine-API section covering what the two agents had to discover by reflection.

### I. Hand-off hygiene
- Briefs name the exact reference sections, and agents read only those.
- A brief never contradicts a skill rule: the planting tool is stated once, in phase 6.

### Memory
**Add, as feedback:**
- adoption models the whole application layer, application services included, never flattened;
- adoption turns the opt-in rules on by default, and exceptions need a reason;
- Codebase Structure mirrors the solution;
- judge a run against the old skill's output on the same code, not against its own report;
- skill fixes come from run evidence, with no version stamps or "expect engine bugs" text.

## Verification
- Read-through.
- Grep the skill for these, and confirm none remain:
  - the "stay off" text;
  - "HTTP surface";
  - Visual Studio `"false"`;
  - the log-line instruction;
  - internal names.
- Every `optin/` id appears in the engine README.
- **On your next run,** compare the new model with the old skill's model of ClientA's code using the same per-type counts as the table above. The target is parity on Services (Service, Operation, Perform Invocation, Entity Actions, Enum, DTO), Domain (Class, Constructor, Association) and Codebase Structure (Solution, Projects, References), with every remaining difference an Exception with a reason.
