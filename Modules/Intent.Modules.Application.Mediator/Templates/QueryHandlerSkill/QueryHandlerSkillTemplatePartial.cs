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

namespace Intent.Modules.Application.Mediator.Templates.QueryHandlerSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryHandlerSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Mediator.QueryHandlerSkill";

        public const string SkillName = "mediator-query-handler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryHandlerSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            var consolidated = ExecutionContext.ConsolidateIntoSingleFile();
            var handlerExample = consolidated
                ? "the *QueryHandler class inside a *Query.cs file"
                : "*QueryHandler.cs";
            var handlerFiles = consolidated
                ? "`*Query.cs` files (each query's `*QueryHandler` class is in the query's own file)"
                : "`*QueryHandler.cs` files";
            var fileLayoutRule = consolidated
                ? "\n- Each query, its handler and its validator share one file, the query's own `*Query.cs`. Keep them there: never move the handler or validator into a file of its own."
                : "";
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($$"""
---
name: {{SkillName}}
description: Implements the whole body of a Mediator query handler from the interactions modelled on the query. MUST load before creating, editing or revisiting any query handler e.g. {{handlerExample}}, and after any Software Factory run that generates a new query handler stub.
template-id: {{TemplateId}}
---

# Mediator Query Handler

Write the `Handle` body of a query handler. The generator only emits a stub that throws `NotImplementedException`; the associations modelled on the query in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for {{handlerFiles}} whose `Handle` still throws `NotImplementedException`, and write every one.
- When you add, remove, reorder or change an association on a query, update its handler in the same piece of work. No generated file changes, so nothing else will remind you.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Never change its name, return type or parameters, and keep it `async` even when it awaits nothing.{{fileLayoutRule}}
- The handler implements `IQueryHandler<TQuery, TResponse>` from the `Mediator` library and returns `ValueTask<TResponse>`.
- Change only what the task needs. Leave the rest of the file as it is, and keep unrelated refactors out.
- Start by reading the handler, its query and response, the repository interfaces and the domain types it reads.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Let the domain model drive the code: use its statuses, identities and business terms rather than names of your own.
- Read through repository interfaces. Filter, sort and page in the query the repository runs, never in memory after loading everything. Project to the response in that query when the repository supports it.
- Apply the same business filters and visibility rules as nearby handlers (for example ownership, tenant, status or soft-delete), and load only what the response needs.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByCriteriaAsync`, `GetSummaryAsync`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Look for similar query handlers, repository methods and projections before writing, and follow the nearest one for paging, sorting, the paged result type, mapping and the not-found pattern. Don't invent a new approach where one exists.
- A query changes no state.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make filtering or mapping easier to read.
- Before finishing, check that every association modelled on the query is implemented, and review your code against every rule in this skill, the sections below included.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the query. Implement its associations in the order the designer shows them; later steps may use earlier results. Re-read them whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which query field filters which attribute, and which attribute fills which response property, by name and meaning. When a name or meaning is ambiguous, check a similar handler before asking.

- Query Entity: load the matching instances through the repository. This is a query's main interaction; shape the response from its result.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets another query, dispatch it with `Mediator.ISender`.
- Create, Update or Delete Entity, Publish Integration Event, Send Integration Command: unusual on a query. Implement what is modelled and tell the user it is unusual.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
