using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.IntegrationTestSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntegrationTestSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.IntegrationTestSkillTemplate";

        internal const string SkillName = "integration-test";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationTestSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;

            var databaseGuidance = UsesInProcessDatabase() ? SqliteDatabaseGuidance : TestcontainersDatabaseGuidance;

            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($"""
---
name: {SkillName}
description: Implement or extend black-box HTTP integration tests for the solution's API, following this codebase's established testing conventions. Use when integration test classes are empty, incomplete, or failing, or when new endpoints need coverage.
template-id: {TemplateId}
---
# API Integration Tests

Implement integration tests that exercise the API end-to-end over HTTP, against the real application host and a real database. Favor testing the observable HTTP contract, self-contained arrangement through the API, and deterministic isolation over shortcuts that couple tests to implementation details.

The generated test project typically already contains the host plumbing: a `WebApplicationFactory` subclass, a database fixture, a shared collection fixture, a `BaseIntegrationTest` exposing `CreateClient()`, and one empty test class per endpoint (marked for merge). Populate those test classes in place; add reusable helpers as new, non-generated files.

## Core rules

- Test through the HTTP boundary ONLY. Send real requests to the endpoints and assert on the `HttpResponseMessage` (status code + deserialized body). How an endpoint is handled internally (mediator, application service, inline logic, CQRS or not) is irrelevant to the test.
- NEVER depend on generated client/proxy classes (e.g. typed `*HttpClient` / `I*Service` proxies). There is no guarantee a given solution has them, and they may be broken. Use a raw `HttpClient` plus the endpoint's own request/response contract types. If such proxy classes exist and prevent the test project from compiling, delete them.
- Depend only on the endpoints and their contracts — the request/response types the endpoints actually accept and return. Do not reach into repositories, handlers, or persistence types to drive a test (reading them to *understand behaviour* is expected; coupling a test to them is not).
- DISCOVER before writing. For every endpoint, read the controller/route AND the code that implements it to learn the exact contract shapes, the success status code, and the exception-to-HTTP-status mapping. Do not assume; the mapping is frequently inconsistent (see the exception-mapping section).
- Assert OBSERVED behaviour, then run the suite to lock it in. Write what the API *actually does* today (so the suite is green and acts as a regression guard), not what it "should" do. Where the real behaviour looks wrong, keep the test green, add a one-line comment, and record it under findings (see the findings section).
- Arrange and assert through the API wherever possible (create a parent via its endpoint, then act). Reserve direct database access for preconditions or assertions the API genuinely cannot express.
- Keep tests DETERMINISTIC and independent (see the isolation section). Never assert on absolute row counts or global ordering that other tests can perturb.
- Populate the existing per-endpoint test classes rather than inventing a parallel structure. Preserve any code-management attributes and the generated class signature; add test methods and usings only.
- Do NOT add test libraries the project does not already reference unless necessary, and never one with a restrictive license (see the assertion section). Reuse what the generated project already brings in.

## Workflow

1. Inspect the generated test project: the `WebApplicationFactory`, the database/container fixture, the shared collection fixture, `BaseIntegrationTest`, and the empty per-endpoint test classes. Confirm what is already wired up.
2. Enumerate the endpoints. For each, run the per-endpoint discovery checklist below.
3. Build a small, self-contained harness as new files (only what's missing): HTTP send/read helpers that never throw on non-success, optional scoped database access, a unique-token generator, and per-slice test-data builders that arrange preconditions through the API.
4. Implement each endpoint's test class: one happy path plus the meaningful edge cases surfaced during discovery (validation → 400, not-found, route/body id conflicts, forbidden/unauthorized, enum/paging bounds).
5. Compile, then RUN the suite against the container. Read the actual status codes and bodies.
6. Stabilise: where an assertion’s expectation differs from observed behaviour, investigate the cause. If it is your test’s mistake, fix the test; if it is the API’s behaviour, assert the observed value, comment it, and add it to findings.
7. Re-run until green. Report the pass count and the findings list.

## Per-endpoint discovery checklist

For each endpoint, extract and note:

- HTTP method + exact route template (including which fields bind from route vs body, and any route/body mismatch handling).
- The exact request contract (constructor/shape, required vs optional, property names and types) and the response contract (type + property names/casing).
- The success status code (do not assume 200/201/204 — verify; e.g. state-changing POSTs may return 201, updates 204, deletes 200).
- Validation rules that produce 400 (which fields, what makes them invalid). Only assert rules that actually exist.
- The behaviour and exact exception type for not-found / business-rule violations, and therefore the resulting HTTP status.
- Preconditions: what must already exist for a valid request, and whether the endpoint returns enough to locate what it created (many create endpoints return 201 with no body/Location — plan to discover the new id via a list/get).

## Test isolation guidance

The test database is normally shared across the whole test collection and is NOT reset between tests, so tests must not assume an empty or fixed dataset.

- Tag every entity a test creates with a unique, collision-free token (e.g. a short GUID-derived string) embedded in a searchable field.
- Scope all queries and assertions to that token (filter list endpoints by it; assert `Single`/`Contains` on it). Never assert on total counts.
- Prefer this over trying to reset the database — the generated fixtures usually don't support a per-test reset, and adding one fights the scaffold.

## Findings report guidance

While writing the tests you will discover real defects and surprises. Do not silently encode a "should be" that fails, and do not hide them. Instead:

- Assert the observed behaviour so the suite stays green.
- Collect the discrepancies and report them to the user (and/or a short README in the test project), each with endpoint, observed vs expected, and likely cause.
- Watch especially for: inconsistent not-found handling (some paths returning 500 instead of 404), missing state-transition guards, thin validation, and query parameters that are effectively required despite having defaults.

## HTTP client guidance

- Use small extension helpers over `HttpClient` that send a request and RETURN the `HttpResponseMessage` without throwing on non-success status codes — integration tests routinely assert on 4xx/5xx, so the status must stay observable. (The framework's throw-on-error convenience methods are unsuitable for the negative cases.)
- Deserialize success bodies with the same serializer settings the server uses (see the serialization section), and expose a helper to read the error/problem body for negative assertions.
- Pass the `TestContext.Current.CancellationToken` variable to HTTP clients where appropriate.

## WebApplicationFactory host guidance

- Reuse the generated factory; obtain clients via `BaseIntegrationTest.CreateClient()`.
- For preconditions/assertions the API cannot express, resolve a scoped service (e.g. the `DbContext`) from the factory's `Services` inside a `using` scope, exactly as a real request would. Detach entities before returning them if lazy-loading proxies are in play.
- Confirm the auth posture: if endpoints are anonymous, no token is needed; if `[Authorize]` (or a fallback policy) applies, arrange an authenticated client and add unauthorized/forbidden cases.

{databaseGuidance}

## Validation guidance

- Determine which validation mechanism the endpoint uses (e.g. FluentValidation validators, DataAnnotations, or none) and assert only the rules that exist.
- For a validation failure, assert the 400 status and, where useful, that the error body identifies the offending field.
- *(This section is composable: use the FluentValidation cues if validators are present; use DataAnnotations cues otherwise; omit if the endpoint has no validation.)*

## JSON & enum serialization guidance

- Configure the test client's serializer to match the server's (for ASP.NET Core defaults: web casing, case-insensitive reads). A mismatch produces false failures that look like behavioural bugs.
- Verify how enums are serialized on the wire (numeric by default unless a string-enum converter is registered) and send/expect them accordingly.
- *(Swap this section for the relevant serializer if the solution does not use System.Text.Json.)*

## Relational / foreign-key guidance

- When a request references another entity by id (foreign key), create that entity for real and use its id. A fabricated/random id can pass in-process validation yet fail at the database FK constraint, surfacing as a 500 rather than a clean 400/404 — arrange the real parent instead.
- *(Applies to relational stores; adjust or omit for document/other stores.)*

## Assertion & test-library guidance

- Use the test framework the generated project already references (commonly xUnit) and its built-in assertions.
- Do NOT add FluentAssertions — recent versions carry a restrictive commercial license unsuitable for a reference/commercial solution. If richer assertions are wanted, prefer a permissively licensed option, but built-in assertions are sufficient.
- Use an already-referenced data-generation library (e.g. AutoFixture) for incidental test data; keep the values that matter to the assertion explicit.

## Output expectations

Produce concrete test code that:

- fills in the empty per-endpoint test classes with a happy path and the relevant edge cases
- adds only the missing, reusable harness/helpers as new files
- arranges preconditions through the API and isolates data with a unique token
- compiles, and passes when run against the generated database fixture
- is accompanied by a findings list for any real defects discovered

## Review checklist

Before finishing, check that:

- no test depends on a generated client/proxy or on internal implementation types
- every list/query request supplies all effectively-required parameters
- entities referenced by id are created for real, not fabricated
- each test is scoped to its own unique token and asserts no global counts
- asserted status codes and bodies match what the running API actually returns
- serializer settings match the server; enums are on the wire in the expected form
- discovered defects are recorded in findings rather than hidden or silently "corrected"

""");
        }

        /// <summary>
        /// True when the EF database fixture runs the database in-process rather than in a container,
        /// which changes what the agent needs from its environment (no container runtime) and what
        /// fidelity it can expect from the database.
        /// </summary>
        private bool UsesInProcessDatabase()
        {
            return ExecutionContext.InstalledModules.Any(p => p.ModuleId == EntityFrameworkCoreModuleId)
                   && ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsSQLLite();
        }

        private const string EntityFrameworkCoreModuleId = "Intent.EntityFrameworkCore";

        private const string TestcontainersDatabaseGuidance = """
## Testcontainers database guidance

- The database fixture (container image, credentials, schema creation) is generated — do not re-implement it. Ensure the container runtime (e.g. Docker) is available before running.
- Test parallelization is typically disabled because one database is shared; keep it that way.
- *(Swap this section if the solution uses a different container/database or an in-memory provider.)*
""";

        private const string SqliteDatabaseGuidance = """
## SQLite database guidance

- The database fixture is generated — do not re-implement it. It runs SQLite **in-process, in memory**, so there is NO container runtime to start: do not install, launch, or wait on Docker, and do not report a missing container runtime as the cause of a failure.
- The fixture owns a `SqliteConnection` that is held open for its lifetime, and hands that live connection to EF Core. An in-memory SQLite database exists only while a connection to it is open. Never close, dispose, or replace that connection from a test, and never re-register the `DbContext` with a connection string of your own — either will silently drop the schema and every subsequent query fails with "no such table".
- Schema is created via `EnsureCreated()`, NOT by running migrations. Anything expressed only in a migration (raw SQL, seed scripts, provider-specific DDL) will not be present.
- Test parallelization is typically disabled because one database is shared; keep it that way.
- SQLite is not a faithful stand-in for SQL Server or PostgreSQL. Expect real behavioural differences in schemas, `decimal` precision (SQLite stores it as `REAL`, so comparisons and ordering can drift), `DateTimeOffset`, computed columns, sequences, and some FK/constraint enforcement. When a test fails in a way that looks like a provider quirk rather than an API defect, say so in findings instead of contorting the API or the test to match SQLite.
""";

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}