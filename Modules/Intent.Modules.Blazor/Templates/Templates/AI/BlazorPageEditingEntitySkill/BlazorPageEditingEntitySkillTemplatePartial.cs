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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageEditingEntitySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorPageEditingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageEditingEntitySkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorPageEditingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-editing-entity")
                .FromMarkdown("""
---
name: blazor-page-editing-entity
description: Creates standard Blazor edit or update entity pages using Bootstrap 5 forms and native Blazor inputs, preserving existing .razor.cs loading, service, and navigation behavior while wiring a valid save flow and model-bound UI. Use when implementing edit, update, or modify entity pages in a standard Blazor (non-MudBlazor) app.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `edit-entity-sample.razor`
2. `edit-entity-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles (`ux-tokens.css`).

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Edit or update entity pages in standard Blazor with Bootstrap 5
Do NOT use for: Search pages, add pages, MudBlazor projects, or non-Blazor projects
Source of truth: Existing `.razor.cs` file defines data loading, service calls, navigation, and model structure

### You MUST NOT:
- Modify existing backend methods such as `UpdateEntity()` or `UpdateEntityAsync()`
- Change payload shape sent to the backend
- Add, rename, or remove model properties
- Invent fields or lookup services
- Rewrite existing C# functionality
- Put C# logic in the `.razor` file using `@code`
- Use MudBlazor components, `.mud-*` selectors, or `--mud-*` variables

---

## 1. Data Loading And Form

Load data through existing lifecycle methods and backing methods such as `OnInitializedAsync()`, `OnParametersSetAsync()`, or existing load methods.

Build the form only from the existing `model` structure. Wrap the fields in an `EditForm` bound to the model with a `DataAnnotationsValidator`, gated behind an `@if (Model is not null)` guard. Show a loading state (a Bootstrap `spinner-border`) while the model is null.

Nullable objects:
- Render conditional sections only when supported by the existing model and state
- Toggle OFF should set the object to null when that pattern already exists
- Toggle ON should initialize it if null
- Keep the exact conditional logic from `.razor.cs`

---

## 2. Map Properties To Bootstrap / Native Controls

| Property Type | Control | Class |
|---------------|---------|-------|
| String | `InputText` (or `InputTextArea`) | `form-control` |
| Number | `InputNumber` | `form-control` |
| Boolean | `InputCheckbox` | `form-check-input` (wrap in `form-check form-switch` for a switch) |
| Enum | `InputSelect` | `form-select` |
| Lookup | `InputSelect` using real loaded options | `form-select` |
| Date | `InputDate` | `form-control` |
| Array | Repeatable Bootstrap rows | `row g-3` |

Control rules:
- Add a placeholder `<option value="">Select...</option>` to nullable lookups
- For dependent lookups (for example sub-category depending on category), use the existing change handler via the `@bind-Value:after` or an explicit `@onchange`-backed method that already exists in `.razor.cs`
- Keep label, input, and `<ValidationMessage>` grouped per field

Enum rules:
- Locate and read the real enum definition before using it
- Use only verified enum members and values
- Never copy enum members from sample code without verification

---

## 3. Validation

Use valid Blazor `EditForm` patterns with `DataAnnotationsValidator` and the model's data-annotation attributes.

Required fields must have:
- Validation attributes or existing validator wiring
- `<ValidationMessage For="..." />` next to each field, and a `<ValidationSummary />` where the sample uses one
- Save gated by `OnValidSubmit`

If multiple forms are used, they must not be nested.

---

## 4. Save Flow

The Save button must call `Save()` or `SaveAsync()` (typically via `EditForm OnValidSubmit`).

That flow must:
1. Run through `EditForm` validation
2. Call the existing update method without modifying it
3. Navigate on success using an existing navigation method when one exists

Forbidden:
- Calling services directly from the Razor template
- Modifying existing update methods
- Changing request payloads

---

## 5. Child Collections

Render child collections in repeatable Bootstrap rows.

Buttons:
- Add buttons only if matching `AddX()` or `AddXAsync()` methods exist (`btn btn-outline-primary`)
- Remove buttons only if matching `RemoveX()` or `RemoveXAsync()` methods exist (`btn btn-outline-danger`)

Indexed bindings:
- Use `for` loops with `var index = i;`
- Never reference `i` directly in bindings, `@key`, or callbacks

---

## 6. Styling

- Prefer shared utility styles first (Bootstrap 5 classes and `ux-tokens.css` utilities)
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Do not add, remove, or re-vendor Bootstrap assets or stylesheet links
- Match the sample layout closely

**Design context (if design.md is present)**

If a `design.md` file exists in the project, read it before choosing Bootstrap classes and utilities. Use it for:
- Button style preferences (`btn-primary` vs `btn-outline-primary`, gradient vs flat)
- Colour semantics for primary, secondary, and danger actions
- Card and panel treatment (border, shadow, hover behaviour)
- Input style and density

`design.md` informs class choices only — it does not override the sample's layout structure.

---

## Definition of Done

- [ ] All bindings used in `.razor` exist in `.razor.cs`
- [ ] No `@code` block was introduced in `.razor`
- [ ] Data is loaded through existing lifecycle or backing methods
- [ ] A loading state is shown while the model is null
- [ ] Form is wrapped in `EditForm` with `DataAnnotationsValidator` and `OnValidSubmit`
- [ ] Save calls an existing save method, not a service directly
- [ ] Backend update methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Conditional sections follow existing backing logic
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] No MudBlazor components, `.mud-*` selectors, or `--mud-*` variables were used

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
