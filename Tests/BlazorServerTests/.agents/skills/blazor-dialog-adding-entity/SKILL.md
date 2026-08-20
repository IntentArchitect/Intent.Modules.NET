---
name: blazor-dialog-adding-entity
description: Implements Blazor add or create entity dialogs using MudBlazor dialog patterns and valid form submission, preserving existing .razor.cs service behavior while wiring save and cancel correctly. Use when creating or implementing add or create entity dialogs in Blazor, including when an empty or skeleton dialog already exists and needs its razor markup or code-behind filled in.
paths:
contentHash: DF77A85A8B2D76C10202AE600DB19F477A3E4BAFB84365F30B72C810AD40295D
---
## MANDATORY: Read Samples Before Implementation

STOP — you MUST read ALL of the following before writing ANY code:

- *Samples** (in the SAME folder as this SKILL.md):
1. `add-entity-dialog-sample.razor`
2. `add-entity-dialog-sample.razor.cs`
- *Target component and project files:**
1. The target `.razor` and `.razor.cs`
2. Related project files: models, enums, lookups, services
- *Design and styling context** (search the project — these are NOT in the SKILL.md folder):
1. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
2. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

- --

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
- Put C# logic in `.razor` using `@code`
- --

## Assess The .razor.cs Before Writing

Read the existing `.razor.cs` in full. Determine whether it is a **skeleton** (constructor, injections, and empty or stub methods only) or **implemented** (contains real model construction, service calls, or save logic).

- *If skeleton** — scaffold the missing members modelled on the sample `.razor.cs`:
- Add a model field or property (initialized to a new instance) matching the sample pattern
- Add `Save()` / `SaveAsync()` that validates the form, calls the existing create service method, and closes the dialog with `MudDialog.Close(DialogResult.Ok(true))` on success
- Add `Cancel()` that only calls `MudDialog.Cancel()`
- Add lookup loading in `OnInitializedAsync()` only if lookups are required by the model and a matching service exists in the project
- *If implemented** — preserve all existing logic exactly:
- Do NOT modify existing methods, service calls, or payload construction
- Do NOT add, rename, or remove model properties
- Do NOT rewrite existing C# functionality
- --

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

- --

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
- --

## 3. Form And Validation In Dialogs

Use valid Blazor form patterns such as `EditForm` with the component's existing validation conventions.

Required fields must have:

- Existing validation annotations or validator wiring
- Visible validation messages
- No service call when invalid

Save button state:

- Disable Save when the form is invalid
- Disable Save while a save is in progress
- --

## 4. Control Mapping

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with verified numeric values |
| Lookup | `MudSelect` from real option sources only |
| Array | Repeatable MudBlazor blocks |
| Date | `MudDatePicker` |

Enum rules:

- Read and verify the enum definition before rendering options
- Use only verified members
- Prefer explicit numeric values in `MudSelectItem`
- Never assume enum members from the sample files

MudBlazor rules:

- Every `MudDatePicker` must include `Placeholder="Select date"`
- --

## 5. Child Collections

- Add collection buttons only when matching backing methods exist
- Remove collection buttons only when matching backing methods exist
- In `for` loops, use `var index = i;` and never bind directly to `i`
- --

## 6. Styling

- Prefer shared utilities first
- Keep component-specific styles minimal
- Never modify existing shared styles or theme values
- Match the sample dialog layout closely
- Cancel button in DialogActions must use `Variant="Variant.Outlined"` `Color="Color.Secondary"`
- Save button must use `Variant="Variant.Filled"` `Color="Color.Primary"` with `Disabled` bound to the saving flag
- *Design and styling context**

Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

- --

## Definition of Done

- [ ] All bindings used in `.razor` exist in `.razor.cs` (including any members just scaffolded)
- [ ] No `@code` block was introduced in `.razor`
- [ ] Dialog uses `IMudDialogInstance` and modern MudBlazor dialog sections
- [ ] Save closes with `DialogResult.Ok(true)` on success
- [ ] Cancel only cancels the dialog
- [ ] Service methods were not called directly from the template
- [ ] Backend methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Enum options were verified against the real enum definition
- [ ] Every `MudDatePicker` includes `Placeholder="Select date"`
- [ ] Validation prevents service calls when invalid
- [ ] Shared styles were preserved and component styling remained minimal
