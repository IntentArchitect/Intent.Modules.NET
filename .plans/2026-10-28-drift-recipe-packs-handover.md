# Handover: Option 2, tested recipe files for common stacks (2026-10-28)

Context for picking up the discussion of the Guardrails drift check's "option 2". Built on 2026-10-28 as six technology guides beside the configure-drift-check skill; the Domain module's CONTEXT.md ("Technology guides") records the design.

## Where things stand

- Committed on `development`:
  - `9e7cb1524a`: only unmodelled public members are reported, `async void` (Task Type `void`), and Service Proxy `Operation Configuration` and `Parameter Modifier`.
  - `8bf1e998e3`: the ValidateDriftRecipes wrong-element warnings, and its `fact`, `element` and `file` filters.
- Both Guardrails Contracts modules are at the unpublished `1.0.0-pre.2`.
- 142 tests pass on Windows and in a Linux container with `TF_BUILD` set.
- The Services module builds with 8 CS0108 warnings that predate this work (commit `956c8c3931`).

## The problem option 2 addresses

The user asked whether an AI in the wild can configure the drift check reliably, given how much implementations vary. The assessment:

| Part | Depends on AI configuration? | Reliability in the wild |
| --- | --- | --- |
| Structure (types and members against the code) | No, it's template-driven | High |
| Facts (database, routes, binding, security, read through recipes) | Entirely: an AI writes regex recipes | Moderate on stacks like the skill's examples; poor elsewhere |
| New types (`own` conventions) | Entirely: no convention means nothing is detected | Patchy |

- **Evidence.** There are three codebases (Basislink, eShop3, eShop4), all .NET and EF Core, each run with a frontier model and with the author fixing the module between runs.
  - The Basislink adversarial test missed 4 of 30 planted changes and gave one false contradiction.
  - The re-test had 3 failures caused by the agent's own recipes.
  - Basislink ended with 26 Services exceptions, many for the wrong reasons.
- **What varies in the wild:**
  - Defaults that depend on context: nullable reference types decide whether `string` is required, and the database provider decides the default text type.
  - Global conventions, such as `configurationBuilder.Properties<string>().HaveMaxLength(256)`.
  - Loops over `GetEntityTypes()`, `ApplyConfigurationsFromAssembly`, shared base configurations and extension helpers.
  - Minimal-API route groups built across files, API versioning, Carter, fallback authorization policies, global filters and custom permission attributes.
- **The incentive problem.** A run fails until it's green. Exceptions, `unreadable`, `default` and `absent` all turn a row off, so an agent under pressure weakens the check.
- **What validation catches now.**
  - The probe catches a case that reads no code.
  - `wrongElementWarnings` catches a case that reads a sibling element's code.
  - The filters show every reading for one kind of fact, element or file.
- **What validation still misses.** A recipe that reads the wrong thing in a way that agrees with the model, an `unstated` assumption that doesn't fit the codebase, and values from `default` or `absent`, which have no code to probe.

## Option 2 as proposed

Ship tested recipe files for common stacks. The AI starts from one and adapts it, instead of writing regexes from scratch, so variance moves from "every AI invents" to "every AI adapts". The module stays technology-agnostic: a pack is recipe data, not engine code, which keeps the user's decision that "the module knows no technology; the recipes do".

Candidate stacks:
- **EF Core:** fluent `OnModelCreating`, `IEntityTypeConfiguration<T>`, data annotations, and mixes of these.
- **Variants:** nullable reference types on and off; SQL Server and PostgreSQL.
- **Endpoints:** ASP.NET Core controllers with attribute routing (including the `[controller]` token), minimal APIs (route groups are hard), FastEndpoints.
- **Others:** Mongo class maps, Cosmos.

## What exists to build on

- **`configure-drift-check`'s Examples section** (`Templates/ModelSyncGuidance.cs`, `ConfigureDriftCheckSkill`, about 27 KB, a file shared by both modules) has worked recipes for:
  - ASP.NET Core controllers;
  - FastEndpoints;
  - EF Core database-first in one `OnModelCreating`;
  - EF Core with `IEntityTypeConfiguration<T>`;
  - a Mongo class map and a Cosmos container's partition key.
  
  `GuidanceTests.Every_JSON_example_in_configure_drift_check_is_a_valid_recipe` parses them all.
- **Basislink's recipe files** (EF Core database-first, and controllers) were checked with a temporary harness against Basislink's real code. The Domain `CONTEXT.md` has more ("Basislink's recipes").
- **The recipe format:**
  - find steps (`class`, `method`, `property`, `parameter`, `statement`, `chain`, `file`, `with <regex>`, `>`), and several find paths as an array;
  - read cases (`match`, `value`, `case`, `part`, `in`, `unreadable`);
  - `inherit`, `default`, `absent`, `unstated` and `replace`;
  - placeholders with filters (`{Service|trimEnd:Controller}`);
  - `own` conventions and `exceptions`.
  
  There is one recipe per kind of fact per designer file (`guardrails/domain.drift.json`, `guardrails/services.drift.json`), and no include or merge mechanism today.
- **Validation:** the `ValidateDriftRecipes` module task (probe, `wrongElementWarnings`, filters with full `readings`) and the `validate-drift-recipes` skill (§4 covers the targeted form).
- **Test helpers:** `BasislinkRegressionTests`, `RecipeCheckTests`, the `Drift.cs` helpers (`Column`, `Table`, `Operation`, `Parameter`, `Fact`) and `Fakes.cs`.

## Questions to unpack

1. **Delivery.** Where do packs live: in the skill text, in files the module writes into the application, or as resources the AI copies from? Packs inside the skill would grow `configure-drift-check`.
2. **Selection.** Does the AI detect the stack (packages in the csproj, patterns in the code), or is it chosen explicitly? How are mixed stacks handled?
3. **Composition.** A codebase mixing fluent configuration and annotations needs several find paths in one recipe. Are packs combinable, or pre-combined variants? Does the format need an include?
4. **Variants that change defaults.** Nullable reference types on or off (`column.required`'s `unstated`), the provider (`column.textType`'s default), and EF Core version conventions.
5. **Testing packs.** A reference codebase per stack, regression tests in the style of `BasislinkRegressionTests`, and who maintains them as frameworks change.
6. **What stays AI-authored.** `own` conventions, exceptions, and house styles: base controllers, custom attributes, helpers.
7. **Skill flow.** `configure-drift-check` would become: identify the stack, start from its pack, adapt it, then validate (no probe or wrong-element warnings, plus a targeted check of each adapted recipe).

## Constraints and user decisions to respect

- The module knows no technology; recipes do. The drift check only checks what is modelled, and never needs a row, recipe or exception for something the designer can't express.
- Recipe validation is an on-demand module task that returns data only. There is no developer-facing coverage summary (the user rejected it on 2026-10-28) and no Recipe check table in the report (removed 2026-10-22).
- One skill per trigger; vary behaviour through task arguments, not extra skills.
- No steering lines that tell the AI to report gaps; fix gaps in the module.
- Size budgets: the Services reverse-engineer skill is about 48 KB rendered and at its limit; `configure-drift-check` is about 27 KB.
- Release notes stay feature-level, about three lines per version.
