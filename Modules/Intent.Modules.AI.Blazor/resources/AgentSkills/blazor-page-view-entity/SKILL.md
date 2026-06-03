---
name: blazor-page-view-entity
description: Creates Blazor read-only view entity pages using MudBlazor layout, preserving existing .razor.cs data loading, service, and navigation behavior while rendering a structured non-editable detail view. Use when implementing view, detail, inspect, or read-only display entity pages in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `EntityViewTemplate.razor`
2. `EntityViewTemplate.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as DTOs, enums, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## MANDATORY: Match Sample Layout (Visual Structure)

When a sample exists, you MUST match the sample's visual structure, not only its data-loading behavior.

Required process:

1. Reuse the sample's top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
2. If the sample uses shared utility classes (e.g. `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (typically under `wwwroot`, such as `ux-theme.css`) and then reuse them.

Required baseline layout (when supported by the target app):

- A hero header using `MudPaper` with `Class="pa-4 mb-4 ux-gradient-primary"` and `Elevation="0"`
- A main content card using `MudCard` with `Class="ux-fade-in-up"` and `Style="animation-delay: 0.1s"`

Forbidden:

- Replacing the hero header with a different structure (e.g. `MudCardHeader`) unless explicitly requested
- Dropping the sample's utility classes when they exist in the target project

---

## Preserve Existing Implementation

Use for: Read-only view or detail entity pages in Blazor with MudBlazor  
Do NOT use for: Add or edit forms, search pages, dialogs, or non-Blazor projects  
Source of truth: Existing `.razor.cs` file defines data loading, service calls, navigation, and the DTO structure

### You MUST NOT:
- Modify existing backend methods such as `LoadEntity()` or `LoadEntityAsync()`
- Change service calls or their parameters
- Add, rename, or remove DTO properties
- Invent fields or services not present in the backing class
- Rewrite existing C# functionality
- Put C# logic in the `.razor` file using `@code`
- Render editable inputs — this page is read-only

---

## 1. Data Loading

Load data through existing lifecycle methods and backing methods such as `OnInitializedAsync()`, `OnParametersSetAsync()`, or explicit load methods already present in `.razor.cs`.

Show a loading indicator while the data object is null, using the same pattern as the sample (`MudProgressCircular` inside a centered `MudStack`).

Do not display entity content until the data object is non-null.

---

## 2. Map DTO Properties to Read-Only Display

Render all scalar DTO properties as labeled read-only fields using `MudText`:

| Property Type | Display |
|---------------|---------|
| String / number | `MudText Typo="Typo.body1"` with `MudText Typo="Typo.overline"` label above |
| Boolean / status | `MudChip` with conditional `Color` and `Variant` |
| Enum | Resolved display label (not raw integer) in `MudChip` or `MudText` |
| Nullable object | Conditional section rendered only when non-null |
| Collection | Iterated with `@foreach`, each item in a `MudPaper` card |

Chip rules:
- Use `T="string"` on every `MudChip`
- Match `Color` semantics: `Color.Success` for positive/active, `Color.Default` or `Color.Error` for inactive/negative
- Match `Variant` semantics: `Variant.Filled` for active, `Variant.Outlined` for inactive or neutral

Enum rules:
- Locate the real enum definition before referencing it
- Use verified member names only — never copy from sample code without confirming they match the target enum

---

## 3. Nullable Object Sections

Render optional nested objects inside an `@if` guard matching the existing null check in the backing class.

Do not render nested fields outside their guard block.

---

## 4. Child Collections

Render child collections with `@foreach` inside a guarded `@if` block.

- When the collection has items, render each in a `MudPaper` card (outlined, rounded) using `MudGrid`
- When the collection is empty or null, render a `MudText` fallback message using `Color.Secondary`
- Never use `for` loops with index variables for read-only collections — `@foreach` is correct here

---

## 5. Navigation Actions

Render navigation buttons at the bottom of the card in a right-aligned `MudStack`:

- Render an **Edit** button only when a matching edit route or method exists in `.razor.cs`
- Always render a **Back** button that calls the existing `Cancel()` or equivalent navigation method
- Never invent navigation methods or routes

Button placement:
- Use `Justify="Justify.FlexEnd"` on `MudStack`
- `Row="true"`, `Spacing="2"`, `AlignItems="AlignItems.Center"`

---

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles, variables, or theme values
- Match the sample layout without introducing unnecessary wrappers
- If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app's styles (usually under `wwwroot`) and reuse them

---

## Definition of Done

- [ ] All properties displayed in `.razor` exist on the DTO returned by the backing class
- [ ] No `@code` block was introduced in `.razor`
- [ ] No editable inputs (e.g. `MudTextField`, `MudSelect`) were rendered
- [ ] Data is loaded through existing lifecycle or backing methods only
- [ ] A loading state is shown while the data object is null
- [ ] Nullable object sections are guarded by `@if` matching existing backing logic
- [ ] Child collections use `@foreach` with an empty-state fallback message
- [ ] Enum values were verified against the real enum definition before use
- [ ] Navigation buttons call only existing methods or use existing routes
- [ ] Shared styles were preserved and component styling remained minimal
- [ ] Sample visual structure was matched (hero header + main card)
- [ ] Sample utility classes were verified to exist in the target project and reused when available
