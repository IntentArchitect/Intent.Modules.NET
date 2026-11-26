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
description: Use when writing or fixing HTTP integration tests in the *.IntegrationTests project, including filling in the generated empty test class for each endpoint.
template-id: {TemplateId}
---
# API Integration Tests

Test the API end to end over HTTP, against the real application host and a real database.

The generated test project already has the host: a `WebApplicationFactory` subclass, a database fixture, a shared collection fixture, a `BaseIntegrationTest` with `CreateClient()`, and one empty test class per endpoint. Fill in those classes, and put reusable helpers in new files.

## Rules

- Test through HTTP only. Send requests with the client from `CreateClient()`, using the endpoint's own request and response types, and assert the status code and the deserialized body.
- Don't use generated service proxies (`*HttpClient` or `I*Service` types in the test project). If they break the build, turn off the `Generate Service Proxies for Testing` application setting and regenerate; a deleted proxy file comes back on the next run.
- Keep each test class's generated signature and code-management attributes. Add test methods and usings only.
- Before testing an endpoint, read its controller and the code behind it for the route, the contracts, the validation rules that return 400, the success status and how exceptions map to statuses. Creates often return 201 with no body, so find the new id through a list or get endpoint; not-found can surface as 500.
- Assert what the API does today, so the suite stays green. Where that looks wrong, comment the assertion and add it to your findings: endpoint, observed, expected, likely cause.
- The database is shared across the collection and never reset. Tag what each test creates with a unique token in a searchable field, scope queries and assertions to it, and never assert total counts or global order.
- Arrange through the API. When a request references another entity by id, create that entity through its endpoint first: a made-up id fails the foreign key and returns 500. Use the database directly, through a scoped `DbContext` from the factory's `Services`, only for what the API can't express.
- Use helpers that return the `HttpResponseMessage` without throwing on error statuses. Deserialize with the server's settings: web casing, and enums as numbers unless a string-enum converter is registered.
- Pass `TestContext.Current.CancellationToken` to async calls.
- Use the test framework's built-in assertions. Don't add FluentAssertions, whose license is commercial, or any library the project doesn't already reference.
- If endpoints require authorization, arrange an authenticated client and add unauthorized and forbidden cases.

{databaseGuidance}

When the tests compile, run them, fix the mistakes that are yours, and report the pass count and your findings.
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
## Database: Testcontainers

- The generated fixture runs the database in a container; don't re-implement it. Docker must be running.
- Parallel test execution is off because the database is shared. Leave it off.
""";

        private const string SqliteDatabaseGuidance = """
## Database: in-memory SQLite

- The generated fixture runs SQLite in process, in memory. There is no container, so don't start or wait for Docker, and don't blame a missing container runtime for a failure.
- The fixture holds a `SqliteConnection` open for its lifetime and gives that connection to EF Core. Never close or replace it, and never re-register the `DbContext` with your own connection string: the in-memory database disappears and every query fails with "no such table".
- The schema comes from `EnsureCreated()`, not migrations, so anything that exists only in a migration is missing.
- Parallel test execution is off because the database is shared. Leave it off.
- SQLite behaves differently from SQL Server and PostgreSQL: `decimal` is stored as `REAL`, and `DateTimeOffset`, schemas, computed columns, sequences and some constraints differ. Report a failure that looks like a provider difference as a finding instead of bending the test or the API to fit SQLite.
""";

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
