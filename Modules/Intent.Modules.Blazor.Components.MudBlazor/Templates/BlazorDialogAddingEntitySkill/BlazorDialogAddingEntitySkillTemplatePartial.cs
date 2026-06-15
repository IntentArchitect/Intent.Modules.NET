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

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.BlazorDialogAddingEntitySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorDialogAddingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Components.MudBlazor.BlazorDialogAddingEntitySkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorDialogAddingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-dialog-adding-entity")
                .FromMarkdown("""
---
name: blazor-dialog-adding-entity
description: Creates Blazor add or create entity dialogs using MudBlazor dialog patterns and valid form submission, preserving existing .razor.cs service behavior while wiring save and cancel correctly. Use when implementing add or create entity dialogs in Blazor.
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
1. `add-entity-dialog-sample.razor`
2. `add-entity-dialog-sample.razor.cs`

**Target component and project files:**
3. The target `.razor` and `.razor.cs`
4. Related project files: models, enums, lookups, services

**Design and styling context** (search the project — these are NOT in the SKILL.md folder):
5. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
6. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

---

## Preserve Existing Implementation

Use for: Add or create entity dialogs in Blazor with MudBlazor  
Do NOT use for: Full pages, search pages, edit dialogs, or non-Blazor projects  
Source of truth: Existing `.razor.cs` file defines service calls, dialog behavior, and model structure  
This is a dialog: close or cancel through MudBlazor dialog APIs rather than navigation

### You MUST NOT:
- Modify existing backend methods such as `CreateEntity()` or `CreateEntityAsync()`
- Change payload shape sent to the backend
- Add, rename, or remove model properties
- Invent lookup services
- Rewrite existing C# functionality
- Add page navigation logic to the dialog flow

---

## 1. Dialog Structure

This component is a MudBlazor dialog, not a page.

Dialog rules:
- In `.razor.cs`, use `IMudDialogInstance`
- In `.razor`, use `TitleContent`, `DialogContent`, and `DialogActions`
- Do not use old dialog tags such as `MudDialogTitle`, `MudDialogContent`, or `MudDialogActions`
- For success, close with `MudDialog.Close(DialogResult.Ok(true))`
- For cancel, use `MudDialog.Cancel()`
- The dialog result property name is `Canceled`, not `Cancelled`

If input data is needed, receive it through `[Parameter]` properties or existing project conventions.

---

## 2. Save And Cancel Methods

Implement or use top-level methods for the template:

`Save()` or `SaveAsync()`:
1. Validate the form
2. Call the existing backend method without modifying it
3. On success, close the dialog with a success result
4. On error, keep the dialog open and set existing error state such as `serviceErrors.*`

`Cancel()`:
- Only cancel or close the dialog
- Do not reset model state
- Do not call services

Template bindings:
- Bind Save button to `Save()` or `SaveAsync()`
- Bind Cancel button to `Cancel()`
- Do not call service methods directly from the Razor template

---

## 3. Form And Validation In Dialogs

Use valid Blazor form patterns such as `EditForm` with the component's existing validation conventions.

Required fields must have:
- Existing validation annotations or validator wiring
- Visible validation messages
- No service call when invalid

Save button state:
- Disable Save when the form is invalid
- Disable Save while a save is in progress

---

## 4. Control Mapping

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with verified numeric values |
| Lookup | `MudSelect` from real option sources only |
| Array | Repeatable MudBlazor blocks |

Enum rules:
- Read and verify the enum definition before rendering options
- Use only verified members
- Prefer explicit numeric values in `MudSelectItem`
- Never assume enum members from the sample files

---

## 5. Child Collections

- Add collection buttons only when matching backing methods exist
- Remove collection buttons only when matching backing methods exist
- In `for` loops, use `var index = i;` and never bind directly to `i`

---

## 6. Styling

- Prefer shared utilities first
- Keep component-specific styles minimal
- Never modify existing shared styles or theme values
- Match the sample dialog layout closely

**Design and styling context**

You have already read `design.md` and the CSS files in the mandatory phase above. Apply what you found:

Use `design.md` for:
- Button variant and fill preferences (`Variant.Filled` / `Variant.Outlined`, gradient vs flat)
- `Color` semantics for primary and error actions
- Dialog title treatment (gradient clip text vs plain text)

Use the CSS files for:
- **Tokens** — use `var(--primary)`, `var(--surface-2)`, `var(--text-muted)` etc. in any inline `Style=` attributes; never hardcode hex values
- **Animation utilities** from `ux-base.css` — `.ux-fade-in-up` (`--dur-slow`) and `.ux-fade-in` (`--dur-med`) are available; verify they exist in the project before applying
- **Component and badge utilities** from `ux-components.css` — `.badge-success`, `.badge-danger`, `.badge-warning`, `.badge-info`, `.badge-neutral`, `.alert-danger`, `.alert-success`, `.alert-warning`, and `.btn-*` variants; verify existence before use

These files inform styling choices only — they do not override the sample's layout structure.

---

## Definition of Done

- [ ] All bindings used in `.razor` exist in `.razor.cs`
- [ ] Dialog uses `IMudDialogInstance` and modern MudBlazor dialog sections
- [ ] Save closes with `DialogResult.Ok(true)` on success
- [ ] Cancel only cancels the dialog
- [ ] Service methods were not called directly from the template
- [ ] Backend methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Enum options were verified against the real enum definition
- [ ] Validation prevents service calls when invalid
- [ ] Shared styles were preserved and component styling remained minimal

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
