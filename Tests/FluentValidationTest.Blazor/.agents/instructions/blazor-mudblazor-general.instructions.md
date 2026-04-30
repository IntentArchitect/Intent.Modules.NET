---
description: Instructions for implementing Blazor components with MudBlazor and modern UI best practices.
appliesTo:
  - "**/*.razor"
  - "**/*.razor.cs"
  - "**/*.razor.css"
contentHash: 7881CFCB744AD277F57846623D0A8180B341995F09D8075C913872199D9A462A
---

## Role and Context
You are a senior C# Blazor engineer. Build modern MudBlazor UIs that compile, follow best practices, and preserve existing application behavior.

## Core Rules

### Styling
- Prefer existing shared or global styles before adding new component styles.
- Keep component styling minimal and specific to the component.
- You may add new utility classes, patterns, styles, or theme values when they do not already exist.
- Do not modify, override, or change existing styles, classes, variables, or theme values.

### File Safety
- Read all provided files and understand how they work together before editing.
- Only modify files explicitly allowed for modification.
- Preserve all `[IntentManaged]` attributes on the file, class, and constructor.
- Add all required `using` clauses in `.razor.cs` files and add `@using` directives in `.razor` files only when needed for Razor compilation or type resolution.
- Use existing services when available.

### Blazor Code-Behind
- Treat the `.razor.cs` file as the backing class and source of truth for component state, UI actions, service calls, and navigation.
- Add Razor markup only in `.razor` files.
- Add C# code only in `.razor.cs` files.
- Do not add `@code` blocks to `.razor` files.
- Preserve existing `.razor.cs` code. You may add code, but do not alter existing logic.
- Never add comments.
- Do not show technical IDs such as GUIDs to end users.
- Ensure forms are valid before create, save, or update flows.
- Ensure every binding introduced in `.razor` has a corresponding member or handler in `.razor.cs`.

## UI and Template Rules

### Actions
- The backing class is the source of truth for page actions, service calls, and navigation.
- Create page action buttons only from methods defined on the component backing class, never from navigation items.
- Scan all backing-class instance methods before generating the template.
- Prefer rendering controls for clear action methods such as `NavigateTo*`, `Add*`, `Create*`, `New*`, `Edit*`, `Update*`, `Delete*`, `Remove*`, `View*`, `Open*`, `Search*`, or `Load*`.
- Never bind to a method that does not exist. If intent is unclear, skip the control.
- For row-level actions such as View, Edit, and Delete, check each action independently and render it only when its corresponding method exists.
- Methods such as `Edit*(id)`, `View*(id)`, `NavigateTo*Edit*(id)`, and `NavigateTo*View*(id)` count as valid row actions when they accept an id-like argument.
- If a table row model exposes an ID field and a matching edit method exists, render the Edit row action bound to that existing method.

### Code-Behind Changes
- You may add helper or orchestration methods in `.razor.cs` when they only update component state or call existing methods in the same class.
- New helper methods must not directly call services or navigation APIs when an existing wrapper method already exists.
- Never add new CRUD or navigation action methods such as `AddEntity()`, `EditEntity(id)`, `ViewEntity(id)`, or `DeleteEntity(id)` if they do not already exist.
- Do not change the internals of existing methods that call injected services or perform navigation.
- If a desired UI action would require changing an existing service or navigation method, call that existing method or add a thin wrapper around it instead of changing its internals.
- Do not create wrapper methods for missing CRUD or navigation actions. If those methods do not already exist, omit the corresponding UI buttons.

### Lifecycle
- Load required initial data in `OnInitializedAsync()` or `OnParametersSetAsync()` as appropriate.
- Prefer calling existing load methods such as `LoadCategories()`, `LoadEntityById(Id)`, or `LoadSubCategories(...)`.
- If required load methods do not exist, add new load methods rather than editing existing service methods.

### Layout
- Use the provided sample template as the layout blueprint.
- Preserve the main structure, DOM hierarchy, and CSS class names from the sample when possible.
- Do not add unnecessary top-level wrappers.
- Keep related action buttons grouped in the same action row when the sample does so.
- Choose the list-page pattern that matches the backing class and do not mix them.
- Use `SearchEntityTemplate` for pages with filtering, paging, sorting, and `ServerData` or `LoadServerData(...)` table flow.
- Use `GridEntityTemplate` for pages that load and display a plain collection without filtering, paging, or sorting.

### List Page Patterns
- For simple grid pages without a search or filter form, include both Add and Refresh actions in `CardHeaderContent` when matching methods exist.
- For searchable list pages, keep the Add and Search actions in the card body with the filters and do not move them into `CardHeaderContent`.
- Keep searchable-page action rows structurally aligned to the sample template and do not replace them with new wrapper patterns.
- Keep action buttons left-aligned with `Justify="Justify.FlexStart"`.
- Do not omit a Refresh action when a matching load or refresh method exists.
- If the backing class exposes `LoadServerData(TableState state, CancellationToken cancellationToken)` or paging and sorting request fields, use `MudTable` with `ServerData`, sortable headers, and pager content.
- If the backing class loads a plain collection and has no paging or sorting request model, use a simple `MudTable` with `Items` bound to that collection.
- In the simple grid pattern, do not generate filter controls, `ServerData`, `MudTableSortLabel`, or pager UI.
- In the filtered pattern, do not replace server-data flow with a plain `Items` table.

### Control Selection
1. MudBlazor component
2. Native Blazor input component
3. Native HTML only as a last resort

Use `MudDatePicker` for dates, `MudSwitch` or `MudCheckBox` for booleans, `MudSelect` for enums, and `MudTextField` for text where appropriate.
For MudBlazor generic components (for example `MudSelect`, `MudRadioGroup`, `MudSwitch`, `MudChipSet`), declare `T` explicitly.

### MudBlazor Binding Rules
- For enum options in `MudSelect`, bind each option value to the enum's numeric value using an explicit cast rather than a string literal.
- Prefer `MudSelect T="int"` with `MudSelectItem T="int" Value="@((int)MyEnum.Value)"` for enum selections.
- Bind MudBlazor component enum properties such as `AlignItems`, `Justify`, `Direction`, `Variant`, `Color`, and `Size` using explicit enum values, not strings.
- Use values such as `AlignItems="AlignItems.Center"`, `Justify="Justify.SpaceBetween"`, and `Direction="FlexDirection.Row"`.

### Template Safety
- Ensure all bindings between `.razor` and `.razor.cs` are valid and the code compiles.
- Ensure lambdas and event callback signatures are valid for the target component.
- Prefer simpler valid Blazor patterns when uncertain.
- In collection rendering with `for` loops, do not reference `i` directly inside bindings, `@key`, or event callbacks.
- In `for` loops, assign `var index = i;` and use `index` throughout the rendered block.

## Navigation Rules
- Navigation items are only for navigation drawers or menus, never for page action buttons.
- Render only the provided navigation items.
- If a matching navigation method exists in the backing class, bind it with `OnClick`; otherwise use appropriate Blazor navigation markup.
- Include icon and display text when the design pattern supports them.
- Do not modify existing navigation methods.
- If a navigation item points to an Add page and the backing class already has a matching action method, create the page button from the method, not from the navigation item.

## Architecture
- Keep components focused on presentation and orchestration.
- Delegate business logic and data access to services.
- Follow Blazor lifecycle best practices for initialization and parameter-driven loading.
- Keep Razor templates and code-behind implementations aligned so bindings remain valid and maintainable.

## Validation Checklist
- [ ] All bindings and event handlers used in `.razor` exist in `.razor.cs`.
- [ ] No `@code` blocks were added to `.razor` files.
- [ ] `[IntentManaged]` attributes are preserved.
- [ ] Required `using` directives were added and the code compiles.
- [ ] No comments were added.
- [ ] Existing global styles and theme values were not changed.
- [ ] Component styles remain minimal and component-specific.
- [ ] Forms are validated for create, save, and update flows.
