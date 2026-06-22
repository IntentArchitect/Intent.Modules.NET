---
name: blazor-page-search-entity
description: Creates Blazor search and list entity pages using MudBlazor tables with optional filtering, preserving existing .razor.cs search, paging, sorting, service, and navigation behavior. Use when implementing search, list, filter, lookup, or query entity pages in Blazor.
paths:
contentHash: 9E736D8C4037D2FEBE57DDD13AC641C5D874FA67ACA5899F8879B32B5DF5B193
---
## MANDATORY: Read Samples Before Implementation

STOP — you MUST read ALL of the following before writing ANY code:

- *Samples** (in the SAME folder as this SKILL.md):
1. `search-entity-sample.razor`
2. `search-entity-sample.razor.cs`
- *Target component and project files:**
1. The target `.razor` and `.razor.cs`
2. Related project files: request models, DTOs, enums, lookups, services
- *Design and styling context** (search the project — these are NOT in the SKILL.md folder):
1. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
2. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

- --

## MANDATORY: Match Sample Layout (Visual Structure)

When a sample exists, you MUST match the sample’s visual structure, not only its data-loading behavior.

Required process:

1. Reuse the sample’s top-level component layout (hero header + main card) unless the user explicitly requests otherwise.
2. If the sample uses shared utility classes (e.g. `ux-gradient-primary`, `ux-fade-in-up`), verify they exist by grepping for the class name as a **substring** (e.g. `ux-gradient-primary`) across all CSS files under `wwwroot` (including `ux-tokens.css`, `ux-base.css`, and `ux-components.css`). CSS utility classes are often defined as compound selectors (e.g. `.mud-paper.ux-gradient-primary`), so search for the class name alone, not the full selector. If the class name appears anywhere in any CSS file, it exists and must be used.

Required baseline layout (when supported by the target app):

- A hero header using `MudPaper` with `Class="pa-4 mb-4 ux-gradient-primary"` and `Elevation="0"`
- A main content card using `MudCard` with `Class="ux-fade-in-up"` and `Style="animation-delay: 0.1s"`

Forbidden:

- Replacing the hero header with a different structure (e.g. `MudCardHeader`) unless explicitly requested
- Dropping the sample’s utility classes when they exist in the target project
- --

## Preserve Existing Implementation

Use for: Search or list entity pages in Blazor with MudBlazor\
Do NOT use for: Add or edit forms, dialogs, or non-Blazor projects\
Source of truth: Existing `.razor.cs` file defines search criteria, paging, sorting, service calls, row actions, and navigation

### You MAY add:

- UI-only fields and helper methods that only call existing methods
- Lifecycle wiring such as `OnInitializedAsync()` calls
- Table columns and row action buttons for existing backing methods

### You MUST NOT:

- Rewrite service-calling, routing, or dialog methods
- Modify backend DTOs, request models, or service signatures
- Invent filters that do not exist in the backing search model
- Expose paging or sorting parameters as normal filter inputs
- Add CRUD or navigation methods that do not already exist in `.razor.cs`
- --

## 1. Filters: Backend Contract Only

All search criteria must come from the existing backing search model or request object.

Required process:

1. Identify the primary search or load method such as `LoadServerData`, `LoadEntities`, or `SearchEntities`
2. Inspect the request DTO or backing search model used by that method
3. Render only supported filter properties

Forbidden:

- Inventing filters
- Modifying service signatures to support UI filters
- Rendering paging or sorting fields like `pageNo`, `pageSize`, or `orderBy` as normal filter inputs
- --

## 2. Choose The Correct Pattern

There are two list-page patterns and you must choose the one that matches the backing class.

Use the searchable pattern when the component exposes:

- `LoadServerData(TableState state, CancellationToken cancellationToken)`
- Paging or sorting request fields
- Real search or filter properties

Use the simple grid pattern when the component:

- Loads a plain collection directly
- Has no paging or sorting request model
- Has no real search or filter fields

Do not mix the two patterns in one page.

- --

## 3. Map Criteria And Fields To MudBlazor Controls

| Type                                  | Control                            |
| ------------------------------------- | ---------------------------------- |
| `string` named like search or keyword | Single search `MudTextField`       |
| Other `string`                        | `MudTextField`                     |
| `bool` or nullable bool               | `MudSelect` with All, Yes, and No  |
| Enum or lookup                        | `MudSelect` with real options only |
| Number                                | `MudNumericField`                  |
| Date                                  | `MudDatePicker`                    |

MudBlazor rules:

- Declare `T` explicitly for generic controls when required
- Add placeholders to `MudSelect`
- If using `ValueChanged`, pair it with `Value` rather than `@bind-Value`
- Bind enum values numerically, not as string literals
- Enum component properties such as `Justify`, `AlignItems`, `Direction`, `Variant`, and `Color` must use explicit enum values
- --

## 4. Search And Refresh Behavior

Search behavior:

- Search button must call the existing load or search method
- Pressing Enter in the main search field should trigger the same search behavior
- Do not auto-query on every keystroke unless that behavior already exists and must be preserved

Button placement:

- With filter fields, keep Search and Add actions inline in the card body with the filters
- Without filter fields, place Add and Refresh actions in `CardHeaderContent` using `MudStack` with `Row="true"` and `Justify="Justify.FlexStart"`
- Keep action buttons left-aligned
- Never use `CardHeaderActions` — it right-aligns content by default

Refresh behavior:

- If a load or refresh method exists, surface a Refresh action
- In simple grid pages, Refresh should call the direct load method
- --

## 5. Table Output And Row Actions

Columns:

- Render only fields that actually exist on the returned DTO or view model
- Never invent columns

Searchable pattern:

- Use `MudTable` with `ServerData` when `LoadServerData` exists
- Use sortable headers only when sorting is supported
- Use pager content only when paging is supported

Simple grid pattern:

- Bind `Items` to the existing collection
- Do not use `ServerData`, sortable headers, or pager content

Row actions:

- Inspect all existing methods on the backing component, not only public methods
- Render View, Edit, Delete, Open, or similar row actions only when matching methods actually exist
- If the row DTO exposes an ID field and a matching edit method exists, the Edit action is required
- Never invent row action methods or placeholder buttons
- --

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles or theme values
- Match the sample layout without introducing unnecessary wrappers
- If the sample uses shared utility classes (for example `ux-gradient-primary`, `ux-fade-in-up`), verify they exist in the target app’s styles (usually under `wwwroot`) and reuse them
- *Design and styling context**

You have already read `design.md` and the CSS files in the mandatory phase above. Apply what you found:

Use `design.md` for:

- Button variant and fill preferences (`Variant.Filled` / `Variant.Outlined`, gradient vs flat)
- `Color` semantics for primary, secondary, and error actions
- Card elevation and hover behaviour
- Page header treatment (gradient clip text vs plain text, icon badge style)

Use the CSS files for:

- **Tokens** — use `var(--primary)`, `var(--surface-2)`, `var(--text-muted)` etc. in any inline `Style=` attributes; never hardcode hex values
- **Animation utilities** from `ux-base.css` — `.ux-fade-in-up` (`--dur-slow`) and `.ux-fade-in` (`--dur-med`) are available; verify they exist in the project before applying
- **Component and badge utilities** from `ux-components.css` — `.badge-success`, `.badge-danger`, `.badge-warning`, `.badge-info`, `.badge-neutral`, `.alert-danger`, `.alert-success`, `.alert-warning`, and `.btn-*` variants; verify existence before use

These files inform styling choices only — they do not override the sample’s layout structure.

- --

## Definition of Done

- [ ] All filters come from the real backing search model or request DTO
- [ ] The correct pattern was chosen: searchable table or simple grid
- [ ] Columns represent only actual DTO fields
- [ ] Search button calls an existing load or search method
- [ ] Refresh is surfaced when a matching method exists
- [ ] Row-level actions are rendered only for existing matching methods
- [ ] No CRUD or navigation methods were invented in `.razor.cs`
- [ ] Enum values and select options were verified against real types
- [ ] Paging and sorting were kept in table flow rather than exposed as normal filter inputs
- [ ] Shared styles were preserved and component styling remained minimal
- [ ] Sample visual structure was matched (hero header + main card), not replaced with an alternative header structure unless explicitly requested
- [ ] Sample utility classes were verified to exist in the target project and reused when available (e.g. grep for `ux-gradient-primary` and `ux-fade-in-up`)
