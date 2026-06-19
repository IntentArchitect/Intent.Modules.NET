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
description: Creates Blazor add or create entity pages using MudBlazor forms, preserving existing .razor.cs service and navigation behavior while wiring a valid save flow and model-bound UI. Use when implementing add, create, new, insert, or register entity pages in Blazor.
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
1. `add-entity-sample.razor`
2. `add-entity-sample.razor.cs`

**Target component and project files:**
3. The target `.razor` and `.razor.cs`
4. Related project files: models, enums, lookups, services

**Design and styling context** (search the project — these are NOT in the SKILL.md folder):
5. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
6. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

---

## MANDATORY: Match Sample Layout (Visual Structure)

When a sample exists, you MUST match the sample's visual structure, not only its data behavior.

Required process:

1. Reuse the sample's top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
2. If the sample uses shared utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`), verify they exist by grepping for the class name as a **substring** across all CSS files under `wwwroot` (including `ux-tokens.css`, `ux-base.css`, and `ux-components.css`). CSS utility classes are often defined as compound selectors, so search for the class name alone. If the class name appears anywhere in any CSS file, it exists and must be used.

Forbidden:

- Replacing the hero header with a different structure unless explicitly requested
- Dropping the sample's utility classes when they exist in the target project

---

## Assess The .razor.cs Before Writing

Use for: Add or create entity pages in Blazor with MudBlazor
Do NOT use for: Search or list pages, edit forms, dialogs, or non-Blazor projects

Read the existing `.razor.cs` in full. Determine whether it is a **skeleton** (constructor, injections, and empty or stub methods only) or **implemented** (contains real model construction, service calls, or save logic).

**If skeleton** — scaffold the missing members modelled on the sample `.razor.cs`:
- Add a model field or property (initialized to a new instance) matching the sample pattern
- Add a `Save()` / `SaveAsync()` method that validates the form, calls the existing create service method, and navigates on success
- Add supporting lookup loading in `OnInitializedAsync()` if lookups are required and the service exists in the project
- Add add/remove collection item methods only when they exist in the sample and matching service methods exist

**If implemented** — preserve all existing logic exactly:
- Do NOT modify existing methods, service calls, or payload construction
- Do NOT add, rename, or remove model properties
- Do NOT rewrite existing C# functionality

**Always forbidden** (skeleton or implemented):
- Inventing service classes or interfaces that don't exist in the project
- Inventing fake option data or hardcoded list values
- Calling services directly from the `.razor` file
- Putting C# logic in `.razor` using `@code`

---

## 1. Form: Build From The Model

Bind inputs only to properties that exist on `model` — adding the model field first if it does not yet exist in the skeleton.

Nullable objects:
- Render a toggle or checkbox section only when the target model supports that nullable object pattern
- Toggle OFF should set the object to null
- Toggle ON should initialize it if null
- Do not render nested fields as always visible or required unless the existing backing model already requires that behavior

---

## 2. Map Properties to MudBlazor Controls

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` or `MudTextField Lines="..."` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with explicit numeric values |
| Lookup | `MudSelect` using real service-loaded options only |
| Array | Repeatable MudBlazor blocks |

Enum rules:
1. Locate the enum definition from imports or project search
2. Read the enum file and verify exact member names and values
3. Use only verified enum members
4. For `MudSelect`, prefer numeric values with explicit casts such as `Value="@((int)AddressType.Delivery)"`
5. Never invent enum members from samples

For MudBlazor generic controls such as `MudSelect`, declare `T` explicitly when required.

---

## 3. Form Validation

Use valid Blazor form patterns with `EditForm` and the component's existing validation conventions.

Required fields must have:
- Model validation attributes or existing validator wiring
- Validation components such as `DataAnnotationsValidator` when the component pattern uses them
- Visible validation messages when invalid

Save button state:
- Disable Save when the form is invalid
- Disable Save when a save operation is already in progress, for example `isLoading`

---

## 4. Save Flow

The Save button must call `Save()` or `SaveAsync()`. If neither exists in `.razor.cs`, add it following the sample pattern.

That save flow must:
1. Validate before saving
2. Call the existing create service method without modifying it
3. Navigate on success using an existing or added navigation method

Forbidden:
- Calling service methods directly from the Razor template
- Modifying existing service-calling methods
- Changing payload construction

---

## 5. Child Collections

Render child collections in repeatable MudBlazor UI blocks.

Buttons:
- Add buttons only if a corresponding `AddX()` or `AddXAsync()` method already exists
- Remove buttons only if a corresponding `RemoveX()` or `RemoveXAsync()` method already exists
- Do not invent collection manipulation methods

For `for` loops:
- Declare `var index = i;` inside each iteration
- Use `index` for bindings, `@key`, and callbacks
- Never bind or pass `i` directly inside the loop body

---

## 6. Styling

- Prefer existing global or shared utility styles first
- Keep component-specific styles minimal
- You may add new shared utility styles only when a reusable pattern is missing
- Never modify existing shared styles, variables, or theme values
- Match the sample layout closely without introducing unnecessary wrappers

**Design and styling context**

You have already read `design.md` and the CSS files in the mandatory phase above. Apply what you found:

Use `design.md` for:
- Button variant and fill preferences (`Variant.Filled` / `Variant.Outlined`, gradient vs flat)
- `Color` semantics for primary, secondary, and error actions
- Card elevation and hover behaviour
- Form control variant (`Variant.Outlined` / `Variant.Filled` / `Variant.Underline`)

Use the CSS files for:
- **Tokens** — use `var(--primary)`, `var(--surface-2)`, `var(--text-muted)` etc. in any inline `Style=` attributes; never hardcode hex values
- **Animation utilities** from `ux-base.css` — `.ux-fade-in-up` (`--dur-slow`) and `.ux-fade-in` (`--dur-med`) are available; verify they exist in the project before applying
- **Component and badge utilities** from `ux-components.css` — `.badge-success`, `.badge-danger`, `.badge-warning`, `.badge-info`, `.badge-neutral`, `.alert-danger`, `.alert-success`, `.alert-warning`, and `.btn-*` variants; verify existence before use

These files inform styling choices only — they do not override the sample's layout structure.

---

## Definition of Done

- [ ] All bindings used in `.razor` resolve to members in `.razor.cs` (including any members just scaffolded)
- [ ] No `@code` block was introduced in `.razor`
- [ ] Save button calls `Save()` or `SaveAsync()` in `.razor.cs`, not a service directly
- [ ] Existing create service methods were not modified
- [ ] Model properties were not arbitrarily renamed or removed — additions are allowed only when scaffolding a skeleton
- [ ] No service classes or interfaces were invented that don't exist in the project
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] Validation is wired and Save is disabled when invalid or loading
- [ ] Shared styles were preserved and component styling remained minimal
- [ ] Sample visual structure was matched (hero header + main card), not replaced unless explicitly requested
- [ ] Sample utility classes were verified to exist in the target project and reused when available

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}