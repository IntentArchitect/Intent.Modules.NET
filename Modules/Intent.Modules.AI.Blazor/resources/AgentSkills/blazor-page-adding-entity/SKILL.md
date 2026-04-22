---
name: blazor-page-adding-entity
description: Creates Blazor add or create entity pages using MudBlazor forms, preserving existing .razor.cs service and navigation behavior while wiring a valid save flow and model-bound UI. Use when implementing add, create, new, insert, or register entity pages in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `add-entity-sample.razor`
2. `add-entity-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as models, enums, lookups, services, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Add or create entity pages in Blazor with MudBlazor  
Do NOT use for: Search or list pages, edit forms, dialogs, or non-Blazor projects  
Source of truth: Existing `.razor.cs` file defines service calls, navigation, model structure, and save flow

### You MUST NOT:
- Modify existing backend methods such as `CreateEntity()` or `CreateEntityAsync()`
- Change the payload shape sent to the backend
- Add, rename, or remove model properties
- Invent lookup services or fake option data
- Rewrite existing C# functionality
- Add navigation or CRUD methods that do not already exist in `.razor.cs`
- Put C# logic in the `.razor` file using `@code`

---

## 1. Form: Build From Existing Model Only

Bind inputs only to properties that already exist on `model`.

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

The Save button must call an existing save orchestration method such as `Save()` or `SaveAsync()`.

That save flow must:
1. Validate before saving
2. Call the existing backend method without modifying it
3. Navigate on success using an existing navigation method when one exists

Forbidden:
- Calling service methods directly from the Razor template
- Modifying service-calling methods
- Changing payload construction
- Inventing post-save navigation methods

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

---

## Definition of Done

- [ ] All bindings used in `.razor` exist in `.razor.cs`
- [ ] No `@code` block was introduced in `.razor`
- [ ] Save button calls an existing save method, not a service directly
- [ ] Backend service methods were not modified
- [ ] Model properties were not added, removed, or renamed
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] Validation is wired and Save is disabled when invalid or loading
- [ ] Shared styles were preserved and component styling remained minimal
