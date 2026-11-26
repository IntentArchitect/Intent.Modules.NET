using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.QueryHandlerSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryHandlerSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Wolverine.QueryHandlerSkillTemplate";

        internal const string SkillName = "wolverine-query-handler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryHandlerSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($"""
---
name: {SkillName}
description: Implements business logic in Wolverine query handler Handle methods. MUST load before creating or editing any query handler e.g. *QueryHandler.cs.
template-id: {TemplateId}
---

# Wolverine Query Handler

Fill in the `Handle` method of an existing query handler. Its signature is generated; only the body is yours.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- Wolverine handlers are plain classes found by naming convention. Never make one implement a handler interface such as `IRequestHandler`.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, its query and response, the repository interfaces and the domain types it reads, and work out the intended result from their names and from nearby features.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its statuses, identities and business terms rather than names of your own.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything.
- Apply the same business filters and visibility rules as nearby handlers (for example ownership, tenant, status or soft-delete), and load only what the response needs.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ListByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Look for similar query handlers, repository methods and projections before writing, and follow the nearest one for paging, sorting, mapping and the response shape. Don't invent a new approach where one exists.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.
- Before finishing, review your code against every rule in this skill, the sections below included.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
