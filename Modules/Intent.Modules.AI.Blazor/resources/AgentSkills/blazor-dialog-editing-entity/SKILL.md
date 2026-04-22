---
name: blazor-dialog-editing-entity
description: Creates Blazor edit or update entity dialogs using MudBlazor dialog patterns and valid form submission, preserving existing .razor.cs loading and service behavior while wiring save and cancel correctly. Use when implementing edit or update entity dialogs in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `edit-entity-dialog-sample.razor`
2. `edit-entity-dialog-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Edit or update entity dialogs in Blazor with MudBlazor  
Do NOT use for: Full pages, search pages, add dialogs, or non-Blazor projects  
Source of truth: Existing `.razor.cs` file defines data loading, service calls, dialog behavior, and model structure  
This is a dialog: close or cancel through MudBlazor dialog APIs rather than navigation

### You MUST NOT:
- Modify existing backend methods such as `UpdateEntity()` or `UpdateEntityAsync()`
- Change payload shape sent to the backend
- Add, rename, or remove model properties
- Invent lookup services
- Rewrite existing C# functionality
- Add navigation logic to the dialog flow

---

## 1. Dialog Structure And Data Loading

This component is a MudBlazor dialog, not a page.

Dialog rules:
- In `.razor.cs`, use `IMudDialogInstance`
- In `.razor`, use `TitleContent`, `DialogContent`, and `DialogActions`
- Do not use old dialog tags such as `MudDialogTitle`, `MudDialogContent`, or `MudDialogActions`
- For success, close with `MudDialog.Close(DialogResult.Ok(true))`
- For cancel, use `MudDialog.Cancel()`

Data loading:
- Receive dialog input through `[Parameter]` properties or existing project conventions
- If an ID is passed, load the entity through existing methods
- If a model is passed, prepopulate from that existing input structure
- Do not invent new dialog input contracts

---

## 2. Save And Cancel Methods

`Save()` or `SaveAsync()`:
1. Validate the form
2. Call the existing update method without modification
3. On success, close the dialog with a success result
4. On error, keep the dialog open and set existing error state such as `serviceErrors.*`

`Cancel()`:
- Only cancel or close the dialog
- Do not reset model state
- Do not call services

Template bindings:
- Bind Save button to `Save()` or `SaveAsync()`
- Bind Cancel button to `Cancel()`
- Do not call backend methods directly from the Razor template

---

## 3. Form Validation

Use valid Blazor `EditForm` patterns with the project's existing validation approach.

Required fields must have:
- Existing validation annotations or validator wiring
- Visible validation messages
- No service call when invalid

Save button state:
- Disable Save when the form is invalid
- Disable Save while saving

---

## 4. Control Mapping

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with verified numeric values |
| Lookup | `MudSelect` from real option sources only |
| Array | Repeatable MudBlazor blocks |

MudBlazor rules:
- Declare `T` explicitly for generic controls when required
- Add placeholders to `MudSelect`
- If using `ValueChanged`, pair it with `Value` rather than `@bind-Value`
- Never assume enum members from sample code

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
- [ ] Entity data is loaded or prepopulated using existing patterns only
- [ ] Save closes with `DialogResult.Ok(true)` on success
- [ ] Cancel only cancels the dialog
- [ ] Backend update methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Enum options were verified against the real enum definition
- [ ] Validation prevents service calls when invalid
- [ ] Shared styles were preserved and component styling remained minimal
