---
name: blazor-page-editing-entity
description: Implements Blazor edit or update entity pages using MudBlazor forms, preserving existing .razor.cs loading, service, and navigation behavior while wiring a valid save flow and model-bound UI. Use when creating or implementing edit, update, or modify entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
paths:
- "**/*.razor"
- "**/*.razor.cs"
- "**/design.md"
- "**/ux-tokens.css"
- "**/ux-base.css"
- "**/ux-components.css"
contentHash: 1CDCB98D4321848B57637C4AD4774A4134B937FD0EEAEA1A65AF2F6C85089575
---
# Edit-entity page (MudBlazor)

For edit or update pages. Not for search, add or view pages, or dialogs.

## 1. Read first

1. `edit-entity-sample.razor` and `edit-entity-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
2. The target `.razor` and `.razor.cs`, and the models, enums, lookups and services they use.
3. If present: `design.md` (anywhere in the project) and `wwwroot/ux-tokens.css`, `ux-base.css`, `ux-components.css`. Missing ones don't block.

## 2. Classify the `.razor.cs`

- **Skeleton** (injections, model classes, empty or stub methods): add what the page needs, following the sample's `.razor.cs`: the id `[Parameter]`, the form reference, a saving flag, loading the entity (and any lookups) through the existing service in `OnInitializedAsync`, and a save method that validates, calls the existing update service method, and navigates on success.
- **Implemented** (has loading, service calls or save logic): wire the markup to the existing members and leave them exactly as they are, conditional logic included. You may add a member beside them only to support the markup, such as a validity flag.

## 3. Layout

Copy the sample's structure: hero header, then one main card holding the form, then right-aligned actions. Reuse the sample's utility classes (`ux-gradient-primary`, `ux-fade-in-up`, ...) when they exist: grep each name as a substring across `wwwroot/**/*.css`, since they are often compound selectors.

## 4. Form

Use the sample's form pattern (`MudForm` with `@ref`), one form only (never nested), and bind only to members that exist on the model.

| Property | Control |
|---|---|
| string | `MudTextField` |
| bool | `MudSwitch` or `MudCheckBox` |
| enum | `MudSelect` over the enum's real members: read its definition, never copy members from the sample |
| lookup | `MudSelect` with a placeholder, over options loaded from an existing service |
| date | `MudDatePicker` with `Placeholder="Select date"` |
| collection | a repeated block per item |

- Declare `T` on generic controls such as `MudSelect` when it can't be inferred.
- With `ValueChanged`, bind `Value`, not `@bind-Value`. Keep `Dense`, `Variant` and `Margin` consistent across the form.
- A nullable nested object goes behind a toggle: off sets it to `null`, on creates it if `null`, and its fields show only while it is on.
- Required fields carry validation and show a message when invalid; validation delegates have correct signatures and nullability.
- Save is disabled while the form is invalid and while a save is running.

## 5. Collections

- In a skeleton, add `AddX` / `RemoveX` methods for the model's collections, as the sample does. In an implemented code-behind, show an add or remove button only when the matching method already exists.
- In a `for` loop, declare `var index = i;` first and use `index`, never `i`, for bindings, `@key` and callbacks.

## 6. Never

- An `@code` block, or a service call, in the `.razor`.
- Changing existing loading or service calls, request payloads, or model properties.
- Inventing services, interfaces, enum members or option data.
- Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

## Definition of Done

- [ ] Hero header + main card as in the sample, with its utility classes where they exist
- [ ] Every binding resolves to a `.razor.cs` member; no `@code`, no service calls in `.razor`
- [ ] The entity loads through the existing service; Save validates, updates through the existing service method, and navigates on success
- [ ] Save is disabled while invalid or saving; required fields show validation
- [ ] Every `MudSelect` has a `Placeholder`; every `MudDatePicker` has `Placeholder="Select date"`
- [ ] Enum members, lookups and services are real
- [ ] Collection buttons only with backing methods; loops use `var index = i`
- [ ] Implemented code-behind: existing members unchanged
- [ ] Styles use tokens; shared styles unchanged
- [ ] The project builds; `.razor` has an `@using` for every type it names, such as a DTO in `T="..."`
