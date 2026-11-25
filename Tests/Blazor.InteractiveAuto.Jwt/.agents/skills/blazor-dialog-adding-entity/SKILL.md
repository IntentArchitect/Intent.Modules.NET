---
name: blazor-dialog-adding-entity
description: Implements Blazor add or create entity dialogs using MudBlazor dialog patterns and valid form submission, preserving existing .razor.cs service behavior while wiring save and cancel correctly. Use when creating or implementing add or create entity dialogs in Blazor, including when an empty or skeleton dialog already exists and needs its razor markup or code-behind filled in.
paths:
contentHash: 33D0452D132393B9A1338447D3C67667D88C4E8A9D09E1F0BFD0B5DE8264C30B
---
# Add-entity dialog (MudBlazor)

For add or create dialogs. Not for pages or edit dialogs. A dialog closes through the dialog instance, never by navigating.

## 1. Read first

1. `add-entity-dialog-sample.razor` and `add-entity-dialog-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
2. The target `.razor` and `.razor.cs`, and the models, enums, lookups and services they use.
3. If present: `design.md` (anywhere in the project) and `wwwroot/ux-tokens.css`, `ux-base.css`, `ux-components.css`. Missing ones don't block.

## 2. Classify the `.razor.cs`

- **Skeleton** (injections, model classes, empty or stub methods): add what the dialog needs, following the sample's `.razor.cs`: the `[CascadingParameter] IMudDialogInstance`, the form reference, a saving flag, lookup loading in `OnInitializedAsync` when the model needs lookups and a service exists, `Save`/`SaveAsync` and `Cancel`.
- **Implemented** (has service calls or save logic): wire the markup to the existing members and leave them exactly as they are. You may add a member beside them only to support the markup, such as a validity flag.

## 3. Dialog

- Markup: `<MudDialog>` with `TitleContent`, `DialogContent` and `DialogActions`; never the old `MudDialogTitle` / `MudDialogContent` / `MudDialogActions` tags.
- Save validates the form, calls the existing create method, closes with `Close(DialogResult.Ok(true))` on success, and on error stays open and sets the existing error state.
- Cancel only cancels the dialog: no service calls, no model reset.
- The result's property is `Canceled`, not `Cancelled`.
- Inputs arrive through `[Parameter]` properties.

## 4. Form

Use the sample's form pattern (`MudForm` with `@ref`) and bind only to members that exist on the model.

| Property | Control |
|---|---|
| string | `MudTextField` |
| bool | `MudSwitch` or `MudCheckBox` |
| enum | `MudSelect` with explicit values over the enum's real members: read its definition, never copy members from the sample |
| lookup | `MudSelect` over options loaded from an existing service |
| date | `MudDatePicker` with `Placeholder="Select date"` |
| collection | a repeated block per item |

- Required fields carry validation and show a message; an invalid form never reaches the service.
- Save is disabled while the form is invalid and while a save is running.
- Actions: Cancel is `Variant="Variant.Outlined" Color="Color.Secondary"`; Save is `Variant="Variant.Filled" Color="Color.Primary"`.
- Collection add and remove buttons only when matching methods exist. In a `for` loop, declare `var index = i;` first and use `index`, never `i`.

## 5. Never

- An `@code` block, or a service call, in the `.razor`.
- Navigation in the dialog flow.
- Changing existing methods, payload shape or model properties; inventing services, enum members or option data.
- Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

## Definition of Done

- [ ] `IMudDialogInstance` and the `TitleContent` / `DialogContent` / `DialogActions` sections, laid out as in the sample
- [ ] Every binding resolves to a `.razor.cs` member; no `@code`, no service calls in `.razor`
- [ ] Save validates, creates through the existing method, and closes with `DialogResult.Ok(true)`; Cancel only cancels
- [ ] Save is disabled while invalid or saving; invalid input never reaches the service
- [ ] Every `MudDatePicker` has `Placeholder="Select date"`; enum members, lookups and services are real
- [ ] Implemented code-behind: existing members unchanged
- [ ] Styles use tokens; shared styles unchanged
- [ ] The project builds; `.razor` has an `@using` for every type it names, such as a DTO in `T="..."`
