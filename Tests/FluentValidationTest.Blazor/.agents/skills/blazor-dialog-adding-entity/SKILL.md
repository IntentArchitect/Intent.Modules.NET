---
name: blazor-dialog-adding-entity
description: Creates Blazor add or create entity dialogs using MudBlazor dialog patterns and valid form submission, preserving existing .razor.cs service behavior while wiring save and cancel correctly. Use when implementing add or create entity dialogs in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
contentHash: 89EB09EDAEB496F0F68729B9C7FCEB9DC7636C368304165A40D761695CD18114
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `add-entity-dialog-sample.razor`
2. `add-entity-dialog-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

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
