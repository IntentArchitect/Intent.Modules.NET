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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageViewEntitySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorPageViewEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageViewEntitySkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorPageViewEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-view-entity")
                .FromMarkdown("""
---
name: blazor-page-view-entity
description: Creates standard Blazor read-only view entity pages using Bootstrap 5 layout, preserving existing .razor.cs data loading, service, and navigation behavior while rendering a structured non-editable detail view. Use when implementing view, detail, inspect, or read-only display entity pages in a standard Blazor (non-MudBlazor) app.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `EntityViewTemplate.razor`
2. `EntityViewTemplate.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as DTOs, enums, and shared styles (`ux-tokens.css`).

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## MANDATORY: Match Sample Layout (Visual Structure)

When a sample exists, you MUST match the sample's visual structure, not only its data-loading behavior.

Required process:

1. Reuse the sample's top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
2. If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (typically under `wwwroot`, such as `ux-tokens.css`) and then reuse them.

Required baseline layout (when supported by the target app):

- A hero header using a `<div>` with `class="p-4 mb-4 ux-gradient-primary text-white rounded"`
- A main content card using `<div class="card ux-fade-in-up">` with a `card-body`

Forbidden:

- Replacing the hero header with a different structure unless explicitly requested
- Dropping the sample's utility classes when they exist in the target project
- Using MudBlazor components, `.mud-*` selectors, or `--mud-*` variables

---

## Preserve Existing Implementation

Use for: Read-only view or detail entity pages in standard Blazor with Bootstrap 5
Do NOT use for: Add or edit forms, search pages, MudBlazor projects, or non-Blazor projects
Source of truth: Existing `.razor.cs` file defines data loading, service calls, navigation, and the DTO structure

### You MUST NOT:
- Modify existing backend methods such as `LoadEntity()` or `LoadEntityAsync()`
- Change service calls or their parameters
- Add, rename, or remove DTO properties
- Invent fields or services not present in the backing class
- Rewrite existing C# functionality
- Put C# logic in the `.razor` file using `@code`
- Render editable inputs — this page is read-only

---

## 1. Data Loading

Load data through existing lifecycle methods and backing methods such as `OnInitializedAsync()`, `OnParametersSetAsync()`, or explicit load methods already present in `.razor.cs`.

Show a loading indicator while the data object is null, using the same pattern as the sample (a Bootstrap `spinner-border` inside a centered `d-flex`).

Do not display entity content until the data object is non-null.

---

## 2. Map DTO Properties to Read-Only Display

Render all scalar DTO properties as labeled read-only fields:

| Property Type | Display |
|---------------|---------|
| String / number | Value text with a small muted label above (`text-uppercase small text-muted`) |
| Boolean / status | Bootstrap `badge` with conditional colour |
| Enum | Resolved display label (not raw integer) in a `badge` or text |
| Nullable object | Conditional section rendered only when non-null |
| Collection | Iterated with `@foreach`, each item in a bordered card (`card` / `border rounded p-3`) |

Badge rules:
- Use semantic colour classes: `bg-success` for positive/active, `bg-secondary` for inactive/neutral, `bg-danger` for negative
- Use outlined styling (`border` + `text-*`) for neutral/inactive states where the sample does

Enum rules:
- Locate the real enum definition before referencing it
- Use verified member names only — never copy from sample code without confirming they match the target enum

---

## 3. Nullable Object Sections

Render optional nested objects inside an `@if` guard matching the existing null check in the backing class.

Do not render nested fields outside their guard block.

---

## 4. Child Collections

Render child collections with `@foreach` inside a guarded `@if` block.

- When the collection has items, render each in a bordered card (`border rounded p-3`) using a Bootstrap `row`
- When the collection is empty or null, render a muted fallback message (`text-muted`)
- Never use `for` loops with index variables for read-only collections — `@foreach` is correct here

---

## 5. Navigation Actions

Render navigation buttons at the bottom of the card in a right-aligned `d-flex justify-content-end gap-2`:

- Render an **Edit** button only when a matching edit route or method exists in `.razor.cs` (`btn btn-primary`)
- Always render a **Back** button that calls the existing `Cancel()` or equivalent navigation method (`btn btn-outline-secondary`)
- Never invent navigation methods or routes

---

## 6. Styling

- Prefer shared utility styles first (Bootstrap 5 classes and `ux-tokens.css` utilities)
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Do not add, remove, or re-vendor Bootstrap assets or stylesheet links
- Match the sample layout without introducing unnecessary wrappers
- If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (usually under `wwwroot`) and reuse them

**Design context (if design.md is present)**

If a `design.md` file exists in the project, read it before choosing Bootstrap classes and utilities. Use it for:
- Button style preferences (`btn-primary` vs `btn-outline-primary`, gradient vs flat)
- Colour semantics for primary, secondary, and danger actions
- Card and panel treatment (border, shadow, hover behaviour)
- Page header treatment (gradient banner vs plain text, icon badge style)
- Badge colour semantics for status display

`design.md` informs class choices only — it does not override the sample's layout structure.

---

## Definition of Done

- [ ] All properties displayed in `.razor` exist on the DTO returned by the backing class
- [ ] No `@code` block was introduced in `.razor`
- [ ] No editable inputs (`InputText`, `InputSelect`, etc.) were rendered
- [ ] Data is loaded through existing lifecycle or backing methods only
- [ ] A loading state is shown while the data object is null
- [ ] Nullable object sections are guarded by `@if` matching existing backing logic
- [ ] Child collections use `@foreach` with an empty-state fallback message
- [ ] Enum values were verified against the real enum definition before use
- [ ] Navigation buttons call only existing methods or use existing routes
- [ ] Sample visual structure was matched (hero header + main card)
- [ ] Sample utility classes were verified to exist in the target project and reused when available
- [ ] No MudBlazor components, `.mud-*` selectors, or `--mud-*` variables were used

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
