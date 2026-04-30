---
name: blazor-page-search-entity
description: Creates Blazor search and list entity pages using MudBlazor tables with optional filtering, preserving existing .razor.cs search, paging, sorting, service, and navigation behavior. Use when implementing search, list, filter, lookup, or query entity pages in Blazor.
paths:
  - "**/*.razor"
  - "**/*.razor.cs"
---

## MANDATORY: Read Samples Before Implementation

STOP - You MUST read ALL sample files in the SAME folder as this SKILL.md before writing ANY code:

1. `search-entity-sample.razor`
2. `search-entity-sample.razor.cs`

Then read the target component `.razor`, `.razor.cs`, and related project files such as request models, DTOs, enums, lookups, services, and shared styles.

If any sample file cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.

---

## Preserve Existing Implementation

Use for: Search or list entity pages in Blazor with MudBlazor  
Do NOT use for: Add or edit forms, dialogs, or non-Blazor projects  
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

---

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

---

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

---

## 3. Map Criteria And Fields To MudBlazor Controls

| Type | Control |
|------|---------|
| `string` named like search or keyword | Single search `MudTextField` |
| Other `string` | `MudTextField` |
| `bool` or nullable bool | `MudSelect` with All, Yes, and No |
| Enum or lookup | `MudSelect` with real options only |
| Number | `MudNumericField` |
| Date | `MudDatePicker` |

MudBlazor rules:
- Declare `T` explicitly for generic controls when required
- Add placeholders to `MudSelect`
- If using `ValueChanged`, pair it with `Value` rather than `@bind-Value`
- Bind enum values numerically, not as string literals
- Enum component properties such as `Justify`, `AlignItems`, `Direction`, `Variant`, and `Color` must use explicit enum values

---

## 4. Search And Refresh Behavior

Search behavior:
- Search button must call the existing load or search method
- Pressing Enter in the main search field should trigger the same search behavior
- Do not auto-query on every keystroke unless that behavior already exists and must be preserved

Button placement:
- With filter fields, keep Search and Add actions inline in the card body with the filters
- Without filter fields, place Add and Refresh actions in `CardHeaderContent`
- Keep action buttons left-aligned

Refresh behavior:
- If a load or refresh method exists, surface a Refresh action
- In simple grid pages, Refresh should call the direct load method

---

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

---

## 6. Styling

- Prefer shared utility styles first
- Keep component-specific styles minimal
- Never modify existing shared styles or theme values
- Match the sample layout without introducing unnecessary wrappers

---

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
