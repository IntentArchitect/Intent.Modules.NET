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

          ## MANDATORY: Read Samples Before Implementation

          STOP — you MUST read ALL of the following before writing ANY code:

          **Samples** (in the SAME folder as this SKILL.md):
          1. `search-entity-sample.razor`
          2. `search-entity-sample.razor.cs`

          **Target component and project files:**
          3. The target `.razor` and `.razor.cs`
          4. Related project files: request models, DTOs, enums, lookups, services

          **Design and styling context** (search the project — these are NOT in the SKILL.md folder):
          5. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
          6. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

          If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
          If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

          ---

          ## MANDATORY: Match Sample Layout (Visual Structure)

          When a sample exists, you MUST match the sample's visual structure, not only its data-loading behavior.

          Required process:

          1. Reuse the sample's top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
          2. If the sample uses shared utility classes (e.g. `ux-gradient-primary`, `ux-fade-in-up`), verify they exist by grepping for the class name as a **substring** (e.g. `ux-gradient-primary`) across all CSS files under `wwwroot` (including `ux-tokens.css`, `ux-base.css`, and `ux-components.css`). CSS utility classes are often defined as compound selectors, so search for the class name alone, not the full selector. If the class name appears anywhere in any CSS file, it exists and must be used.

          Required baseline layout (when supported by the target app):

          - A hero header `<div class="ux-gradient-primary ux-rounded-lg p-4 mb-4">` with an `<h1>`/`<p>`
          - A main content card `<div class="card ux-fade-in-up">` with a `<div class="card-body">`

          Forbidden:

          - Replacing the hero header with a different structure unless explicitly requested
          - Dropping the sample's utility classes when they exist in the target project

          ---

          ## Preserve Existing Implementation

          Use for: Search or list entity pages in Blazor\
          Do NOT use for: Add or edit forms, dialogs, or non-Blazor projects\
          Source of truth: Existing `.razor.cs` file defines search criteria, paging, sorting, service calls, row actions, and navigation

          ### You MAY add:

          - UI-only fields and helper methods that only call existing methods
          - Lifecycle wiring such as `OnInitializedAsync()` calls
          - Table columns and row action buttons for existing backing methods

          ### You MUST NOT:

          - Rewrite service-calling, routing, or dialog methods
          - Modify backend DTOs, request models, or service signatures
          - Invent filters that do not exist in the backing search model
          - Expose paging or sorting parameters as normal filter inputs
          - Add CRUD or navigation methods that do not already exist in `.razor.cs`

          ---

          ## 1. Filters: Backend Contract Only

          All search criteria must come from the existing backing search model or request object.

          Required process:

          1. Identify the primary search or load method such as `LoadEntities` or `SearchEntities`
          2. Inspect the request DTO or backing search model used by that method
          3. Render only supported filter properties

          Forbidden:

          - Inventing filters
          - Modifying service signatures to support UI filters
          - Rendering paging fields like `pageNo` or `pageSize` as normal filter inputs

          ---

          ## 2. Choose The Correct Pattern

          There are two list-page patterns and you must choose the one that matches the backing class.

          Use the searchable/paged pattern when the component exposes:

          - `PageNo`/`PageSize` backing properties and a load method that requests a paged result (e.g. returns a `PagedResult<T>`)
          - Real search or filter properties

          Use the simple grid pattern when the component:

          - Loads a plain collection directly
          - Has no paging request model
          - Has no real search or filter fields

          Do not mix the two patterns in one page.

          ---

          ## 3. Map Criteria And Fields To Controls

          | Type | Control |
          |------|---------|
          | `string` named like search or keyword | Single search `<input class="form-control">` bound with `@bind`/`@bind:event="oninput"`, with `@onkeydown` triggering search on Enter |
          | Other `string` | Plain `<input class="form-control">` |
          | `bool` or nullable bool | `<select class="form-select">` with All, Yes, and No options |
          | Enum or lookup | `<select class="form-select">` with real options only |
          | Number | `<input type="number" class="form-control">` |
          | Date | `<input type="date" class="form-control">` |

          Rules:
          - Bind filters directly to their backing properties
          - Bind enum/lookup `<option>` values to the real member or id, never a string literal guess

          ---

          ## 4. Search And Refresh Behavior

          Search behavior:

          - Search button must call the existing load or search method (e.g. `ReloadAsync()`)
          - Pressing Enter in the main search field should trigger the same search behavior via `@onkeydown`
          - Do not auto-query on every keystroke unless that behavior already exists and must be preserved

          Button placement:

          - Keep Search and Add actions inline in the card body, left-aligned, using plain buttons (e.g. `btn btn-primary` for Add, `btn btn-outline-primary` for Search)
          - Keep action buttons left-aligned; do not right-align them

          Refresh behavior:

          - If a load or refresh method exists, surface a Refresh action
          - In simple grid pages, Refresh should call the direct load method

          ---

          ## 5. Table Output And Row Actions

          Columns:

          - Render only fields that actually exist on the returned DTO or view model
          - Never invent columns

          Searchable/paged pattern:

          - Render results in a plain `<table class="table">` with `<thead>`/`<tbody>`
          - Use manual Previous/Next paging buttons (`btn btn-outline`, disabled at the bounds) bound to `PageNo`/`PageSize` when paging is supported
          - Only add sortable columns if the backing method actually supports sorting — there is no built-in sortable-header convention in this pattern

          Simple grid pattern:

          - Bind rows from the existing collection directly
          - Do not add paging controls

          Row actions:

          - Inspect all existing methods on the backing component, not only public methods
          - Render View, Edit, Delete, Open, or similar row actions only when matching methods actually exist, using plain buttons (e.g. `btn btn-outline` for View, `btn btn-outline-primary` for Edit, `btn btn-outline-danger` for Delete)
          - If the row DTO exposes an ID field and a matching edit method exists, the Edit action is required
          - Never invent row action methods or placeholder buttons
          - A destructive action such as Delete should confirm before calling the backing method — follow the sample's inline confirmation pattern (an `alert alert-warning` with Confirm/Cancel buttons) rather than a modal dialog, unless the project already has a different confirmation convention

          ---

          ## 6. Styling

          - Prefer shared utility styles first
          - Keep component-specific styles minimal
          - Never modify existing shared styles or theme values
          - Match the sample layout without introducing unnecessary wrappers
          - If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (usually under `wwwroot`) and reuse them

          **Design and styling context**

          Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

          ---

          ## Definition of Done

          - [ ] All filters come from the real backing search model or request DTO
          - [ ] The correct pattern was chosen: searchable/paged table or simple grid
          - [ ] Columns represent only actual DTO fields
          - [ ] Search button calls an existing load or search method
          - [ ] Refresh is surfaced when a matching method exists
          - [ ] Row-level actions are rendered only for existing matching methods
          - [ ] No CRUD or navigation methods were invented in `.razor.cs`
          - [ ] Enum values and select options were verified against real types
          - [ ] Paging was kept as Previous/Next controls bound to `PageNo`/`PageSize` rather than exposed as normal filter inputs
          - [ ] Shared styles were preserved and component styling remained minimal
          - [ ] Sample visual structure was matched (hero header + main card), not replaced with an alternative header structure unless explicitly requested
          - [ ] Sample utility classes were verified to exist in the target project and reused when available (e.g. grep for `ux-gradient-primary` and `ux-fade-in-up`)

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
