---
name: blazor-page-editing-entity
description: Implements Blazor edit or update entity pages using MudBlazor forms, preserving existing .razor.cs loading, service, and navigation behavior while wiring a valid save flow and model-bound UI. Use when creating or implementing edit, update, or modify entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
paths:
contentHash: 12DC75F5303E38469F6F25D7C0966E96A3B45488B367EAD2BB0E98EA4189B7AE
---
## MANDATORY: Read Samples Before Implementation

STOP — you MUST read ALL of the following before writing ANY code:

- *Samples** (in the SAME folder as this SKILL.md):
1. `edit-entity-sample.razor`
2. `edit-entity-sample.razor.cs`
- *Target component and project files:**
1. The target `.razor` and `.razor.cs`
2. Related project files: models, enums, lookups, services
- *Design and styling context** (search the project — these are NOT in the SKILL.md folder):
1. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
2. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

- --

## MANDATORY: Match Sample Layout (Visual Structure)

When a sample exists, you MUST match the sample's visual structure, not only its data behavior.

Required process:

1. Reuse the sample's top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
2. If the sample uses shared utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`), verify they exist by grepping for the class name as a **substring** across all CSS files under `wwwroot` (including `ux-tokens.css`, `ux-base.css`, and `ux-components.css`). CSS utility classes are often defined as compound selectors, so search for the class name alone. If the class name appears anywhere in any CSS file, it exists and must be used.

Forbidden:

- Replacing the hero header with a different structure unless explicitly requested
- Dropping the sample's utility classes when they exist in the target project
- --

## Assess The .razor.cs Before Writing

Use for: Edit or update entity pages in Blazor with MudBlazor
Do NOT use for: Search pages, add pages, dialogs, or non-Blazor projects

Read the existing `.razor.cs` in full. Determine whether it is a **skeleton** (constructor, injections, and empty or stub methods only) or **implemented** (contains real data loading, model construction, or service calls).

- *If skeleton** — scaffold the missing members modelled on the sample `.razor.cs`:
- Add `[Parameter]` properties needed for the page (e.g. entity ID)
- Add a model field or property matching the sample pattern
- Implement `OnInitializedAsync()` or `OnParametersSetAsync()` to load the entity via the appropriate service (search the project for a matching service interface)
- Add a `Save()` / `SaveAsync()` method that validates the form, calls the existing update service method, and navigates on success using an existing or added navigation method
- Add supporting methods (cancel, add/remove collection items) only when they exist in the sample and the relevant service methods exist in the project
- *If implemented** — preserve all existing logic exactly:
- Do NOT modify existing methods, service calls, or payload construction
- Do NOT add, rename, or remove model properties
- Do NOT rewrite existing C# functionality
- *Always forbidden** (skeleton or implemented):
- Inventing service classes or interfaces that don't exist in the project
- Calling services directly from the `.razor` file
- Putting C# logic in `.razor` using `@code`
- --

## 1. Data Loading And Form

If `OnInitializedAsync()` is empty or a stub, implement it to load the entity via the appropriate service method. If the method already has a body, leave it unchanged.

Build the form from the model structure — adding the model field if it does not yet exist in the skeleton.

Nullable objects:

- Render conditional sections only when supported by the existing model and state
- Toggle OFF should set the object to null when that pattern already exists
- Toggle ON should initialize it if null
- Keep the exact conditional logic from `.razor.cs`
- --

## 2. Map Properties To MudBlazor Controls

| Property Type | Control |
|---------------|---------|
| String | `MudTextField` |
| Boolean | `MudSwitch` or `MudCheckBox` |
| Enum | `MudSelect` with verified numeric values |
| Lookup | `MudSelect` using real loaded options |
| Array | Repeatable MudBlazor blocks |
| Date | `MudDatePicker` |

MudBlazor rules:

- Declare `T` explicitly for generic controls when required
- Add placeholders to `MudSelect`
- Every `MudDatePicker` must include `Placeholder="Select date"`
- If using `ValueChanged`, pair it with `Value` rather than `@bind-Value`
- Keep `Dense`, `Variant`, and `Margin` settings consistent across the form when used

Enum rules:

- Locate and read the real enum definition before using it
- Use only verified enum members and values
- Never copy enum members from sample code without verification
- --

## 3. Validation

Use valid Blazor `EditForm` patterns with the project's existing validation approach.

Required fields must have:

- Validation wiring from model annotations or existing validators
- Visible validation messages when invalid
- Save disabled when invalid

If using MudBlazor validation delegates, ensure signatures and nullability are correct.

If multiple forms are used, they must not be nested.

- --

## 4. Save Flow

The Save button must call `Save()` or `SaveAsync()`. If neither exists in `.razor.cs`, add it following the sample pattern.

That flow must:

1. Validate the form
2. Call the existing update service method without modifying it
3. Navigate on success using an existing or added navigation method

Forbidden:

- Calling services directly from the Razor template
- Modifying existing update service methods
- Changing request payloads
- --

## 5. Child Collections

Render child collections in repeatable MudBlazor UI blocks.

Buttons:

- Add buttons only if matching `AddX()` or `AddXAsync()` methods exist
- Remove buttons only if matching `RemoveX()` or `RemoveXAsync()` methods exist

Indexed bindings:

- Use `for` loops with `var index = i;`
- Never reference `i` directly in bindings, `@key`, or callbacks
- --

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Match the sample layout closely
- *Design and styling context**

Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

- --

## Definition of Done

- [ ] All bindings used in `.razor` resolve to members in `.razor.cs` (including any members just scaffolded)
- [ ] No `@code` block was introduced in `.razor`
- [ ] Data is loaded through lifecycle or backing methods (implemented or added if the skeleton had none)
- [ ] Save button calls `Save()` or `SaveAsync()` in `.razor.cs`, not a service directly
- [ ] Existing update service methods were not modified
- [ ] Model properties were not arbitrarily renamed or removed — additions are allowed only when scaffolding a skeleton
- [ ] No service classes or interfaces were invented that don't exist in the project
- [ ] Conditional sections follow the backing logic
- [ ] Child collection buttons exist only when backing methods exist
- [ ] Enum options were verified against the real enum definition
- [ ] Every `MudDatePicker` includes `Placeholder="Select date"`
- [ ] Validation is wired and Save is disabled when invalid or loading
- [ ] Sample visual structure was matched (hero header + main card), not replaced unless explicitly requested
- [ ] Sample utility classes were verified to exist in the target project and reused when available
