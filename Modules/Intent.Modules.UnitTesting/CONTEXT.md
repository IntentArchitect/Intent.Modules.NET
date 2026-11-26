# Context: Intent.UnitTesting

## Purpose

Generates xUnit unit-test stubs for commands, queries, services and operations. It also generates a
`unit-test-guidelines` AI skill for filling them in.

## Architectural Decisions

- **The line is 1.3.0, a minor version (developer decision, 2026-11-26).** It began as 1.2.1; the developer then
  ruled the move to a skill a minor change, because agents now receive the guidance a different way. The migrations
  keep their 1.2.1-pre.0 and 1.2.1-pre.1 versions: the test apps had already run them, and an application upgrading
  from 1.2.0 to 1.3.0 still runs both, since both lie between the two versions.
- **`unit-test-guidelines` is a skill, not an instruction (1.3.0).** Developer decision, November 2026. Before 1.3.0
  it was an always-loaded instruction, so every agent session carried about 1,300 words of unit-test guidance
  whether or not it touched a test. Now it is a skill: an agent loads it only when it works on a test. It was
  briefly kept as a trimmed instruction in this same line before the developer chose to convert it. The trim keeps
  the rules that change what an agent writes in this codebase: code management of the merge-managed test class,
  naming, real entity types and DTO factories, mocking `FindAllAsync` predicates and AutoMapper's singular
  `Map<TDto>`, capturing a created entity's Id, and what to cover per handler kind. The reasoning behind the trimmed
  shape is in `Intent.Application.MediatR`'s `CONTEXT.md`, "Handler skills: trimmed text, no pointer rule".
- **The conversion is a role change in the designer, not a new template.** `UnitTestInstructionTemplate` keeps its
  name, so its template id (`Intent.UnitTesting.UnitTestInstructionTemplate`) doesn't change. Its Template Settings
  role moved from `AI.Context.Instructions` to `AI.Context.Skills`, and it now writes `unit-test-guidelines/SKILL.md`.
  A new template would have meant a new id and an orphaned output in every consumer; renaming the element would have
  changed the id too.
- **The description carries the trigger; no pointer rule.** It is a calm "Use when…" sentence naming the
  `*.UnitTests` project, which the module creates as `{Application project}.UnitTests`. A glob-scoped rule telling
  agents to load the skill was built in this line and removed before release (developer decision): in the evaluation
  (skills inventory report, §17) trimmed skills loaded from their names and descriptions alone. The full reasoning is
  in the MediatR section named above.
- **Domain-service tests don't verify `SaveChangesAsync`.** Developer ruling: a domain service may use repositories
  but never the unit of work. Keep this consistent with `Intent.DomainServices`' skill.

## Invariants & Constraints

- **An upgrade has to move the placed output: `Migration_01_02_01_Pre_00`.** A consumer keeps a template output in the
  folder it was first placed in. Changing the role alone therefore left existing applications writing the skill to
  `.agents/instructions/unit-test-guidelines/SKILL.md`, where harnesses don't look for skills. Seen in
  `IntegrationTesting.Tests` and `IntegrationTesting.SQLLite.Tests`. The version migration finds the first
  `AI.Context.Skills` output anchor in each Codebase Structure package. It moves every
  `Intent.UnitTesting.UnitTestInstructionTemplate` output into that anchor's folder, copying its stereotypes and
  metadata. The persistence API has no move, so the migration removes the element and adds it again; Intent gives the
  re-added output a new id, which is harmless because nothing references a template output by id. If a package has
  no skills anchor, the output stays where it is.
- **An application with several agent harnesses has one output per skills folder: `Migration_01_02_01_Pre_01`.** An
  app can target `.claude`, `.codex`, `.github` and `.opencode` at once, each with its own skills folder and its own
  unit-test output. `Pre_00` stacks all of them in the first skills folder, and two outputs in one folder fail the
  Software Factory with "Duplicate Template Output found at same location" (seen in `TestMediator.NoEventing`).
  `Pre_00` had already been installed in the test apps at 1.2.1-pre.0, and a migration runs only when an application
  upgrades past its version, so the fix is a new migration at 1.2.1-pre.1 rather than an edit to `Pre_00`. `Pre_01`
  keeps the first output in each skills folder, moves each extra one to a skills folder that has none, and removes any
  left over. The outputs are identical apart from their folder, so which output goes to which harness does not matter.
  A 1.2.0 application upgrading to 1.3.0 runs `Pre_00` and then `Pre_01`. Tested in a scratch copy of
  `TestMediator.NoEventing` (four harnesses): from 1.2.0 to 1.2.1-pre.1, and from a stacked 1.2.1-pre.0 state to
  1.2.1-pre.1. Both ended with one output in each of the four skills folders.
- **A migration's version is the module version it shipped in.** Once a version has been installed anywhere, its
  migration must not change: applications already past it never run it again. A change to upgrade behaviour needs a
  new module version and a new Version Migration element in the designer, plus its `<migration>` entry in the
  `.imodspec`.
- **The `.imodspec` is ignored in the Software Factory, so it is maintained by hand.** That covers its `<version>`,
  `<tags>`, `<templates>` (ids and roles) and `<migrations>`, and they must be kept in step with the Module Builder
  model. A Version Migration modelled in the designer runs only once the `.imodspec` lists it under `<migrations>`.
  Without the entry the generated class compiles and ships, but Intent never calls it.
- **The module has no static content template.** The legacy `resources/AIInstructions/unit-test.md`, an untrimmed
  older copy of the guidance, was deleted in 1.2.1: packaging copies `resources/*` into `content/*`, and a static
  content template ships every file in its content folder, so it would have become a second instruction. The unit-test
  guidance lives only in `UnitTestInstructionTemplate`.
- **The mocking examples use Moq syntax**, although the module supports NSubstitute. They were not made
  framework-specific in this trim.

## Module Interactions

- **`Intent.Common`**: written against 3.11.1. The markdown file builder behind the skill is available at that
  version.
- **`Intent.Persistence.SDK`** (1.0.3): `IPersistenceLoader`, used by both migrations to read and save the Codebase
  Structure designer.

## Superseded

- **"Trimmed but stays an instruction" (earlier in this line, as 1.2.1).** This was replaced by the conversion to a skill.
- **"A skill plus a pointer rule" (earlier in this line, as 1.2.1).** The pointer rule was removed before release; see
  Architectural Decisions.
