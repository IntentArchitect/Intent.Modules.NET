# Intent.Common.DriftEngine: plan

Agreed on 2026-11-12. The next step is phase 0, the specification.

## Objective

Give each codebase a visual of its design intent that is guaranteed true in the code.

- The visual shows intention, not everything the code does.
- A drift check flags any stated claim that stops being true, whichever side changed. That keeps agents, and people, honest.
- It works on any codebase in any language.
- Modules extend the engine through code: their own designers, stereotypes, readers and rules. Everything the custom drift modules did (eShopCustom, the Java spike) stays within reach. The engine is the shared foundation, not the ceiling. Reading code is only ever C# in a module (user decision, 2026-11-16: recipes, a second, JSON way, were removed).
- Code generation is only for bootstrapping.

## Principles

Every phase must keep these.

1. Code the model doesn't mention is never drift. The only exception is a rule a team opts into that says something must be modelled, and that rule applies only within scope.
2. Every claim on the visual is checked. A claim that can't be verified is reported as such; nothing modelled is skipped silently.
3. A check passes only when the code really says it.
4. Adopting and checking never write code.
5. New languages, stacks and vocabulary are added as extensions (a module's code, or a pack's data), not by changing the core.
6. Every finding names the claim, the model element and the file and line, so an agent can fix the right side.

## Building blocks

| # | Building block              | What it is                                                                                                                                                                            |
| - | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1 | Open vocabulary             | Any designer, element type or stereotype can carry checkable claims, whether it is stock or a module's own.                                                                           |
| 2 | A code reader per language  | Reads symbols, members, signatures and references the same way for every language. Where the framework can describe itself (EF Core's model, OpenAPI), facts come from the framework. |
| 3 | Describe-only structure     | Compares the types, members and signatures the code reader finds with the model, and writes nothing into the code.                                                                    |
| 4 | Rules beyond find-and-regex | Claim readers and rules in C#, in modules' extensions, over the code graph and the model: one way to read code.                                                                      |
| 5 | Policy in the engine        | Every claim ends as verified, contradicted or can't-verify. Exceptions carry a reason. Severities and scope are set here.                                                             |

### Scope (part of block 5)

Brownfield code is full of junk, so the engine must be told what is in and what is out.

- Scope lives in the model, where it is visible and editable. It lists what's in (projects, folders, files) and what's out (globs, folders, types), and every exclusion has a reason.
- Only in-scope code is read, so a claim can't be satisfied by dead code, tests, generated files or vendored code.
- Rules over the code apply only in scope. That covers layer imports, and any "every X must be modelled" rule a team opts into.
- Scope grows area by area during adoption. The Drift Scope in the model says what is out of scope and why, and the Software Factory log lists it, so the visual's coverage is honest. The drift report holds only drift, so it never lists what is out of scope.
- Scope can't hide drift:
  - A modelled claim is always checked; if its code sits out of scope, that is reported.
  - A scope entry that matches nothing is reported as stale.

### Stack packs

A stack module bundles the readers, vocabulary and conventions for one technology (EF Core, ASP.NET Core, NServiceBus, Spring MVC, JPA, Vue…). A codebase combines the packs it uses and adds one small pack for its house style. Packs are what get pre-built, so reuse comes without losing flexibility.

## What we reuse

- **From Common.Drift:**
  - vocabulary and conventions as data (its recipes were removed, 2026-11-16: reading code is C#);
  - the probe, validation and the wrong-element check;
  - "can't be verified";
  - the AI skills and the report hub.
- **From module building:**
  - designer extensions and module-owned stereotypes;
  - live validation;
  - the deep checks (types, call targets, routes, imports);
  - the Architecture model.

## Decisions

- **A new module, Intent.Common.DriftEngine.** Clients on Common.Drift are migrated later. To keep that migration cheap:
  - keep Common.Drift's kind and placeholder names, so its recipe files map onto stock and house readers;
  - report through Intent.Common.DriftReporting, with the engine's own files, so both engines can run side by side.
- **The second language is Java.**
- **Adopting a real codebase takes 2 hours at most.**

## Phases

Each phase ends with a review. Phases are verified with unit tests, fixtures and builds only.

**The existing evals are not part of this plan.** That includes `Tests.Brownfield` and every agent-driven or smoke suite. Never run them, never plan around them, and never use them as a gate. Whether they are ever run is the user's decision alone.

| Phase                       | Delivers                                                                                                                                                                                                                                                                                               | Done when                                                                                                                                    |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------- |
| 0. Specification            | Requirements; the interfaces between the blocks (claim and finding model, code-reader schema, rule API, pack format); the scope model. An acceptance suite of unit tests and fixtures, built from the 321 checks reduced to intent claims, the real codebases and scope cases. Common.Drift's recipe files must map onto the engine. | Signed off                                                                                                                                   |
| 1. Foundations              | Intent.Common.DriftEngine with blocks 2, 3 and 5, including scope, and a C# reader                                                                                                                                                                                                                     | eShopCustom's structure checks pass with zero hand-written files changed (user decision, 2026-11-13: eShopCustom is the system; basislink5 and CrmBus are examples). Excluded junk is not read, and the report holds only drift. |
| 2. Open vocabulary          | Block 1                                                                                                                                                                                                                                                                                                | A Front End Store and an Architecture Layer are checked with no core change                                                                  |
| 3. Rules beyond regex       | Block 4, piloted on Architecture (layers, May Depend On, imports)                                                                                                                                                                                                                                      | Planted import violations are caught in C# (eShopCustom) and Java (the spike)                                                                |
| 4. Java and the first packs | A Java reader, plus packs for Spring MVC, JPA, EF Core, ASP.NET Core and NServiceBus                                                                                                                                                                                                                   | The Java spike's Domain and Services claims are checked with no Java-specific code in the core                                               |
| 5. Adoption flow            | AI skills that set scope, build a curated intent model, choose packs and verify                                                                                                                                                                                                                        | A real codebase is adopted in 2 hours or less, with no code changes                                                                          |
| 6. Migration (later)        | A converter that maps recipe files onto stock and house readers, and a side-by-side run                                                                                                                                                                                                                | basislink5 and CrmBus give the same results on the new engine                                                                                |

## Evidence and inputs

- **The two module-building implementations:**
  - eShopCustom (`D:\Dev\Temp\eShopCustom\intent\modules`);
  - the Java spike (`D:\Dev\Spikes\Samples.eShop.Demo.Java`).

  Their 321 checks were classified on 2026-11-12. The full classification is in `check-classification.md`, beside this plan. Read that file; don't search old chat transcripts.
- **Real codebases adopted with Common.Drift:**
  - basislink5 (`D:\Dev\Temp\basislink5`);
  - CrmBus.DocumentManagement (`D:\Dev\Clients\CrmBus.DocumentManagement1`, client code, so keep it local).
- **Common.Drift recipes to convert:** basislink5, CrmBus, and eShop10, eShop13 and eShop14.
