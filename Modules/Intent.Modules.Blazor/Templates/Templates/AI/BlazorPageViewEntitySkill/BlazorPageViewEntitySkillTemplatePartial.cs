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
description: Creates Blazor read-only view entity pages using MudBlazor layout, preserving existing .razor.cs data loading, service, and navigation behavior while rendering a structured non-editable detail view. Use when implementing view, detail, inspect, or read-only display entity pages in Blazor.
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
1. `view-entity-sample.razor`
2. `view-entity-sample.cs`

**Target component and project files:**
3. The target `.razor` and `.razor.cs`
4. Related project files: DTOs, enums

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
2. If the sample uses shared utility classes (e.g. `ux-gradient-primary`, `ux-fade-in-up`), verify they exist by grepping for the class name as a **substring** (e.g. `ux-gradient-primary`) across all CSS files under `wwwroot` (including `ux-tokens.css`, `ux-base.css`, and `ux-components.css`). CSS utility classes are often defined as compound selectors (e.g. `.mud-paper.ux-gradient-primary`), so search for the class name alone, not the full selector. If the class name appears anywhere in any CSS file, it exists and must be used.

Required baseline layout (when supported by the target app):

- A hero header using `MudPaper` with `Class="pa-4 mb-4 ux-gradient-primary"` and `Elevation="0"`
- A main content card using `MudCard` with `Class="ux-fade-in-up"` and `Style="animation-delay: 0.1s"`

Forbidden:

- Replacing the hero header with a different structure (e.g. `MudCardHeader`) unless explicitly requested
- Dropping the sample's utility classes when they exist in the target project

---

## Preserve Existing Implementation

Use for: Read-only view or detail entity pages in Blazor with MudBlazor  
Do NOT use for: Add or edit forms, search pages, dialogs, or non-Blazor projects  
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

Show a loading indicator while the data object is null, using the same pattern as the sample (`MudProgressCircular` inside a centered `MudStack`).

Do not display entity content until the data object is non-null.

---

## 2. Map DTO Properties to Read-Only Display

Render all scalar DTO properties as labeled read-only fields using `MudText`:

| Property Type | Display |
|---------------|---------|
| String / number | `MudText Typo="Typo.body1"` with `MudText Typo="Typo.overline"` label above |
| Boolean / status | `MudChip` with conditional `Color` and `Variant` |
| Enum | Resolved display label (not raw integer) in `MudChip` or `MudText` |
| Nullable object | Conditional section rendered only when non-null |
| Collection | Iterated with `@foreach`, each item in a `MudPaper` card |

Chip rules:
- Use `T="string"` on every `MudChip`
- Match `Color` semantics: `Color.Success` for positive/active, `Color.Default` or `Color.Error` for inactive/negative
- Match `Variant` semantics: `Variant.Filled` for active, `Variant.Outlined` for inactive or neutral

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

- When the collection has items, render each in a `MudPaper` with `Class="pa-3 ux-inner-panel"` and `Elevation="0"` using `MudGrid` — do not use `Outlined="true"` or hardcode a `Style` border-radius
- When the collection is empty or null, render a `MudText` fallback message using `Color.Secondary`
- Never use `for` loops with index variables for read-only collections — `@foreach` is correct here

---

## 5. Navigation Actions

Render navigation buttons at the bottom of the card in a right-aligned `MudStack`:

- Render an **Edit** button only when a matching edit route or method exists in `.razor.cs`
- Always render a **Back** button that calls the existing `Cancel()` or equivalent navigation method
- Never invent navigation methods or routes

Button placement:
- Use `Justify="Justify.FlexEnd"` on `MudStack`
- `Row="true"`, `Spacing="2"`, `AlignItems="AlignItems.Center"`

---

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Match the sample layout without introducing unnecessary wrappers
- If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (usually under `wwwroot`) and reuse them

**Design and styling context**

Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

---

## Definition of Done

- [ ] All properties displayed in `.razor` exist on the DTO returned by the backing class
- [ ] No `@code` block was introduced in `.razor`
- [ ] No editable inputs (e.g. `MudTextField`, `MudSelect`) were rendered
- [ ] Data is loaded through existing lifecycle or backing methods only
- [ ] A loading state is shown while the data object is null
- [ ] Nullable object sections are guarded by `@if` matching existing backing logic
- [ ] Child collections use `@foreach` with an empty-state fallback message
- [ ] Enum values were verified against the real enum definition before use
- [ ] Navigation buttons call only existing methods or use existing routes
- [ ] Shared styles were preserved and component styling remained minimal
- [ ] Sample visual structure was matched (hero header + main card)
- [ ] Sample utility classes were verified to exist in the target project and reused when available

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}