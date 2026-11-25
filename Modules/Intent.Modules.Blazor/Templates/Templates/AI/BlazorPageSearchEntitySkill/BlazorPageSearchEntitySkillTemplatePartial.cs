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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageSearchEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorPageSearchEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageSearchEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorPageSearchEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-search-entity")
        .FromMarkdown("""
          ---
          name: blazor-page-search-entity
          description: Implements Blazor search and list entity pages using a plain Bootstrap table with optional filtering, preserving existing .razor.cs search, paging, sorting, service, and navigation behavior. Use when creating or implementing search, list, filter, lookup, or query entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
          paths:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/design.md"
          - "**/ux-tokens.css"
          - "**/ux-base.css"
          - "**/ux-components.css"
          ---

          # Search / list page (Bootstrap)

          For search and list pages. Not for add or edit forms, or dialogs.

          ## 1. Read first

          1. `search-entity-sample.razor` and `search-entity-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the request models, DTOs, enums, lookups and services they use.
          3. `wwwroot/ux-tokens.css`, for the real token names. Missing it doesn't block.

          ## 2. The `.razor.cs` is the source of truth

          It defines the search criteria, paging, sorting, service calls, row actions and navigation.

          - **Skeleton** (injections and empty or stub methods): fill in loading through the existing service, following the sample's `.razor.cs`.
          - **Implemented**: leave every existing method as it is, service-calling, routing and dialog methods included. You may add UI-only fields, helpers that only call existing methods, and lifecycle wiring.
          - Never add missing CRUD or navigation methods, or change DTOs, request models or service signatures.

          ## 3. Pick one pattern, never a mix

          | The code-behind has | Pattern |
          |---|---|
          | `PageNo` / `PageSize`, a paged result (such as `PagedResult<T>`), real filters | **Searchable**: `<table class="table">`, Previous/Next bound to `PageNo` / `PageSize` and disabled at the bounds; sortable columns only if the load method sorts |
          | a plain collection, no paging or filters | **Simple grid**: rows straight from the collection; no paging controls |

          ## 4. Layout

          Copy the sample's structure: hero header (`<div class="ux-gradient-primary ux-rounded-lg p-4 mb-4">` with `<h1>` and `<p>`), then a main card (`<div class="card ux-fade-in-up">` with `card-body`), with no extra wrappers. Keep it unless the user asks otherwise; `design.md` informs styling, not layout. Reuse the sample's utility classes when they exist: grep each name as a substring across `wwwroot/**/*.css`, since they are often compound selectors.

          Actions:
          - Add (`btn btn-primary`) and Search (`btn btn-outline-primary`) sit inline in the card body, left-aligned.
          - Show Refresh, calling the load method, when one exists.

          ## 5. Filters

          Render only properties of the request model the search method takes; never invent filters, and never render paging or sorting fields (`pageNo`, `pageSize`, `orderBy`).

          | Type | Control |
          |---|---|
          | `string` named like search or keyword | one search `<input class="form-control">` with `@bind` and `@bind:event="oninput"`; `@onkeydown` runs the search on Enter |
          | other `string` | `<input class="form-control">` |
          | `bool` / `bool?` | `<select class="form-select">` with All (empty), true and false, bound through a string property like the sample's `IsActiveText` |
          | enum or lookup | `<select class="form-select">` over the real enum members or ids, never guessed strings |
          | number | `<input type="number" class="form-control">` |
          | date | `<input type="date" class="form-control">` |

          - Search calls the existing load method (the sample's `ReloadAsync`); no query per keystroke unless the code already does it.

          ## 6. Columns and row actions

          - Columns only for real DTO fields.
          - Check all code-behind methods, private included. Render View (`btn btn-outline`), Edit (`btn btn-outline-primary`), Delete (`btn btn-outline-danger`) or similar only when a matching method exists; with a row id and an edit method, Edit is required. No placeholder buttons.
          - Delete confirms first with the sample's inline `alert alert-warning` and Confirm/Cancel buttons (not a modal) unless the project has its own convention.

          ## 7. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

          ## Definition of Done

          - [ ] Hero header + main card as in the sample, with its utility classes where they exist
          - [ ] One pattern, matching the code-behind
          - [ ] Filters only from the real request model, with real enum and lookup values; paging fields are never filters
          - [ ] Search and Enter in the search field call the existing method; Refresh shown when a load method exists
          - [ ] Columns are real DTO fields; row actions only for existing methods; Delete confirms inline
          - [ ] No new CRUD or navigation methods; existing methods unchanged
          - [ ] Styles use tokens; shared styles unchanged
          - [ ] The project builds; `.razor` has an `@using` for every type it names

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
