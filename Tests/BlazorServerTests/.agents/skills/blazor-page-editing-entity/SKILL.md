---
name: blazor-page-editing-entity
description: Creates Blazor edit or update entity pages using MudBlazor forms, preserving existing .razor.cs loading, service, and navigation behavior while wiring a valid save flow and model-bound UI. Use when implementing edit, update, or modify entity pages in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
contentHash: DCDABC5A6D3D19023959F857559C8EE88DCB6B7ABE6A424BC3AABE272C46FB97
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `edit-entity-sample.razor`
2. `edit-entity-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Edit or update entity pages in Blazor with MudBlazor  
Do NOT use for: Search pages, add pages, dialogs, or non-Blazor projects  
Source of truth: Existing `.razor.cs` file defines data loading, service calls, navigation, and model structure

### You MUST NOT:
- Modify existing backend methods such as `UpdateEntity()` or `UpdateEntityAsync()`
- Change payload shape sent to the backend
- Add, rename, or remove model properties
- Invent fields or lookup services
- Rewrite existing C# functionality
- Put C# logic in the `.razor` file using `@code`

---

## 1. Data Loading And Form

Load data through existing lifecycle methods and backing methods such as `OnInitializedAsync()`, `OnParametersSetAsync()`, or existing load methods.

Build the form only from the existing `model` structure.

Nullable objects:
- Render conditional sections only when supported by the existing model and state
- Toggle OFF should set the object to null when that pattern already exists
- Toggle ON should initialize it if null
- Keep the exact conditional logic from `.razor.cs`

---

## 2. Map Properties To MudBlazor Controls

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with verified numeric values |
| Lookup | `MudSelect` using real loaded options |
| Array | Repeatable MudBlazor blocks |

MudBlazor rules:
- Declare `T` explicitly for generic controls when required
- Add placeholders to `MudSelect`
- If using `ValueChanged`, pair it with `Value` rather than `@bind-Value`
- Keep `Dense`, `Variant`, and `Margin` settings consistent across the form when used

Enum rules:
- Locate and read the real enum definition before using it
- Use only verified enum members and values
- Never copy enum members from sample code without verification

---

## 3. Validation

Use valid Blazor `EditForm` patterns with the project's existing validation approach.

Required fields must have:
- Validation wiring from model annotations or existing validators
- Visible validation messages when invalid
- Save disabled when invalid

If using MudBlazor validation delegates, ensure signatures and nullability are correct.

If multiple forms are used, they must not be nested.

---

## 4. Save Flow

The Save button must call `Save()` or `SaveAsync()`.

That flow must:
1. Validate the form
2. Call the existing update method without modifying it
3. Navigate on success using an existing navigation method when one exists

Forbidden:
- Calling services directly from the Razor template
- Modifying existing update methods
- Changing request payloads

---

## 5. Child Collections

Render child collections in repeatable MudBlazor UI blocks.

Buttons:
- Add buttons only if matching `AddX()` or `AddXAsync()` methods exist
- Remove buttons only if matching `RemoveX()` or `RemoveXAsync()` methods exist

Indexed bindings:
- Use `for` loops with `var index = i;`
- Never reference `i` directly in bindings, `@key`, or callbacks

---

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Match the sample layout closely

---

## Definition of Done

- [ ] All bindings used in `.razor` exist in `.razor.cs`
- [ ] No `@code` block was introduced in `.razor`
- [ ] Data is loaded through existing lifecycle or backing methods
- [ ] Save button calls an existing save method, not a service directly
- [ ] Backend update methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Conditional sections follow existing backing logic
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] Validation is wired and Save is disabled when invalid or loading
