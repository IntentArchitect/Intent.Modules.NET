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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageAddingEntitySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorPageAddingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageAddingEntitySkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorPageAddingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-adding-entity")
                .FromMarkdown("""
---
name: blazor-page-adding-entity
description: Creates standard Blazor add or create entity pages using Bootstrap 5 forms and native Blazor inputs, preserving existing .razor.cs service and navigation behavior while wiring a valid save flow and model-bound UI. Use when implementing add, create, new, insert, or register entity pages in a standard Blazor (non-MudBlazor) app.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `add-entity-sample.razor`
2. `add-entity-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles (`ux-tokens.css`).

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Add or create entity pages in standard Blazor with Bootstrap 5
Do NOT use for: Search or list pages, edit forms, MudBlazor projects, or non-Blazor projects
Source of truth: Existing `.razor.cs` file defines service calls, navigation, model structure, and save flow

### You MUST NOT:
- Modify existing backend methods such as `CreateEntity()` or `CreateEntityAsync()`
- Change the payload shape sent to the backend
- Add, rename, or remove model properties
- Invent lookup services or fake option data
- Rewrite existing C# functionality
- Add navigation or CRUD methods that do not already exist in `.razor.cs`
- Put C# logic in the `.razor` file using `@code`
- Use MudBlazor components, `.mud-*` selectors, or `--mud-*` variables

---

## 1. Form: Build From Existing Model Only

Bind inputs only to properties that already exist on `model`. Wrap the fields in an `EditForm` bound to the model with a `DataAnnotationsValidator` and `OnValidSubmit` pointing at the existing save method.

Nullable objects:
- Render a toggle or checkbox section only when the target model supports that nullable object pattern
- Toggle OFF should set the object to null
- Toggle ON should initialize it if null
- Do not render nested fields as always visible or required unless the existing backing model already requires that behavior

---

## 2. Map Properties to Bootstrap / Native Controls

| Property Type | Control | Class |
|---------------|---------|-------|
| String | `InputText` (or `InputTextArea` for long text) | `form-control` |
| Number | `InputNumber` | `form-control` |
| Boolean | `InputCheckbox` | `form-check-input` (wrap in `form-check form-switch` for a switch) |
| Enum | `InputSelect` | `form-select` |
| Lookup | `InputSelect` using real service-loaded options only | `form-select` |
| Date | `InputDate` | `form-control` |
| Array | Repeatable Bootstrap rows | `row g-3` |

Layout each field as a Bootstrap column with a `form-label`, the input, and a `<ValidationMessage>`.

Enum rules:
1. Locate the enum definition from imports or project search
2. Read the enum file and verify exact member names and values
3. Use only verified enum members
4. For `InputSelect`, bind each `<option>`'s `value` to the enum member so it round-trips to the enum type
5. Never invent enum members from samples

For nullable lookups, include a placeholder `<option value="">Select...</option>`.

---

## 3. Form Validation

Use valid Blazor form patterns with `EditForm`, `DataAnnotationsValidator`, and the model's data-annotation attributes.

Required fields must have:
- Model validation attributes (for example `[Required]`, `[EmailAddress]`) or existing validator wiring
- `<ValidationMessage For="() => model.Property" />` next to each field
- A `<ValidationSummary />` where the sample uses one

Save button state:
- Use `OnValidSubmit` so the save runs only when the form is valid
- Disable Save when a save operation is already in progress, for example `_saving`

---

## 4. Save Flow

The Save button (a `type="submit"` button inside the `EditForm`, or an explicit handler) must call an existing save orchestration method such as `Save()` or `SaveAsync()`.

That save flow must:
1. Run through `EditForm` validation before saving
2. Call the existing backend method without modifying it
3. Navigate on success using an existing navigation method when one exists

Forbidden:
- Calling service methods directly from the Razor template
- Modifying service-calling methods
- Changing payload construction
- Inventing post-save navigation methods

---

## 5. Child Collections

Render child collections in repeatable Bootstrap rows (for example each item in a `row g-3` separated by a divider).

Buttons:
- Add buttons only if a corresponding `AddX()` or `AddXAsync()` method already exists
- Remove buttons only if a corresponding `RemoveX()` or `RemoveXAsync()` method already exists
- Do not invent collection manipulation methods
- Style buttons with `btn btn-outline-primary` (add) and `btn btn-outline-danger` (remove)

For `for` loops:
- Declare `var index = i;` inside each iteration
- Use `index` for bindings, `@key`, and callbacks
- Never bind or pass `i` directly inside the loop body

---

## 6. Styling

- Prefer existing global or shared utility styles first (Bootstrap 5 classes and `ux-tokens.css` utilities)
- Keep component-specific styles minimal
- You may add new shared utility styles only when a reusable pattern is missing
- Never modify existing shared styles, variables, or theme values
- Do not add, remove, or re-vendor Bootstrap assets or stylesheet links
- Match the sample layout closely without introducing unnecessary wrappers

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
- [ ] Form is wrapped in `EditForm` with `DataAnnotationsValidator` and `OnValidSubmit`
- [ ] Save calls an existing save method, not a service directly
- [ ] Backend service methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] Validation is wired and Save is disabled while saving
- [ ] No MudBlazor components, `.mud-*` selectors, or `--mud-*` variables were used
- [ ] Shared styles were preserved and component styling remained minimal

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
