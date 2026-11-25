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

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.MudBlazorPageSearchEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class MudBlazorPageSearchEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Components.MudBlazor.MudBlazorPageSearchEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public MudBlazorPageSearchEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-search-entity")
        .FromMarkdown("""
          ---
          name: blazor-page-search-entity
          description: Implements Blazor search and list entity pages using MudBlazor tables with optional filtering, preserving existing .razor.cs search, paging, sorting, service, and navigation behavior. Use when creating or implementing search, list, filter, lookup, or query entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
          paths:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/design.md"
          - "**/ux-tokens.css"
          - "**/ux-base.css"
          - "**/ux-components.css"
          ---

          # Search / list page (MudBlazor)

          For search and list pages. Not for add or edit forms, or dialogs.

          ## 1. Read first

          1. `search-entity-sample.razor` and `search-entity-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the request models, DTOs, enums, lookups and services they use.
          3. If present: `design.md` (anywhere in the project) and `wwwroot/ux-tokens.css`, `ux-base.css`, `ux-components.css`. Missing ones don't block.

          ## 2. The `.razor.cs` is the source of truth

          It defines the search criteria, paging, sorting, service calls, row actions and navigation.

          - **Skeleton** (injections and empty or stub methods): fill in loading through the existing service, following the sample's `.razor.cs`.
          - **Implemented**: leave every existing method as it is. You may add UI-only fields, helpers that only call existing methods, and lifecycle wiring.
          - Never add CRUD or navigation methods that aren't there, and never change DTOs, request models or service signatures.

          ## 3. Pick one pattern

          | The code-behind has | Pattern |
          |---|---|
          | `LoadServerData(TableState, CancellationToken)`, paging or sorting fields, real filter properties | **Searchable**: `MudTable` with `ServerData`; sort labels only if sorting is supported, pager only if paging is |
          | a plain collection loaded directly, no paging, sorting or filters | **Simple grid**: `Items` bound to the collection; no `ServerData`, sort labels or pager |

          Don't mix them.

          ## 4. Layout

          Copy the sample's structure: hero header (`MudPaper Class="pa-4 mb-4 ux-gradient-primary" Elevation="0"`), then a main card (`MudCard Class="ux-fade-in-up" Style="animation-delay: 0.1s"`). Don't replace the hero with `MudCardHeader`. Reuse the sample's utility classes when they exist: grep each name as a substring across `wwwroot/**/*.css`, since they are often compound selectors.

          Actions:
          - With filters: Search and Add sit inline with the filters in the card body.
          - Without filters: Add and Refresh go in `CardHeaderContent`, in a `MudStack Row="true" Justify="Justify.FlexStart"`. Never `CardHeaderActions`, which right-aligns.
          - Surface Refresh when a load method exists; in a simple grid it calls that load method.

          ## 5. Filters

          Render only properties of the request model the search method actually takes. Never invent filters, and never render paging or sorting fields (`pageNo`, `pageSize`, `orderBy`) as filters.

          | Type | Control |
          |---|---|
          | `string` named like search or keyword | one search `MudTextField`; Enter triggers the search |
          | other `string` | `MudTextField` |
          | `bool` / `bool?` | `MudSelect` with All, Yes and No |
          | enum or lookup | `MudSelect` with a placeholder, over real values only |
          | number | `MudNumericField` |
          | date | `MudDatePicker` with `Placeholder="Select date"` |

          - The Search button calls the existing load or search method. Don't query on every keystroke unless the code already does.
          - Declare `T` on generic controls when it can't be inferred. With `ValueChanged`, bind `Value`, not `@bind-Value`.
          - Bind enums numerically, and give enum parameters (`Justify`, `Variant`, `Color`, ...) explicit enum values.

          ## 6. Columns and row actions

          - Columns only for fields that exist on the returned DTO.
          - Look at every method on the code-behind, private ones included. Render View, Edit, Delete or similar actions only when a matching method exists; if the row has an id and an edit method exists, the Edit action is required. No placeholder buttons.

          ## 7. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

          ## Definition of Done

          - [ ] Hero header + main card as in the sample, with its utility classes where they exist
          - [ ] One pattern, matching the code-behind
          - [ ] Filters only from the real request model; paging and sorting stay in the table
          - [ ] Search calls the existing method; Refresh is shown when a load method exists
          - [ ] Columns are real DTO fields; row actions only for existing methods
          - [ ] Every `MudSelect` has a `Placeholder`; every `MudDatePicker` has `Placeholder="Select date"`; enum values and options are real
          - [ ] No new CRUD or navigation methods; existing methods unchanged
          - [ ] Styles use tokens; shared styles unchanged
          - [ ] The project builds; `.razor` has an `@using` for every type it names, such as a DTO in `T="..."`

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
