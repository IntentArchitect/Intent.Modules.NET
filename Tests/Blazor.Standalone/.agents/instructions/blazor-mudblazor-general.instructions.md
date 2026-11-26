---
description: Instructions for implementing Blazor components with MudBlazor and modern UI best practices.
appliesTo:
- "**/*.razor"
- "**/*.razor.cs"
- "**/*.razor.css"
contentHash: FF4284F95B1A5DA9CCBE4E1A925F3F2B048660E027C7F11AA43E71549F152B4B
---
## Role and Context

You are a senior C# Blazor engineer. Build modern MudBlazor UIs that compile, follow best practices, and preserve existing application behavior.

## Core Rules

#### Required instructions for the coding subagent

Include these in the coding subagent task: before reading, patching, writing or generating any code, check whether an applicable skill exists and, if one does, call `use_skill` for it first. Only implement directly when no applicable skill exists.

### Styling

- Prefer existing shared or global styles before adding new component styles.
- Keep component styling minimal and specific to the component.
- You may add new utility classes, patterns, styles, or theme values when they do not already exist.
- Do not modify, override, or change existing styles, classes, variables, or theme values.
- Any request to change styling, the theme, colours/branding, typography or `design.md` — a supplied replacement, a drift you spot between `design.md` and the CSS, or a from-scratch styling interview — must go through `use_skill` for `updating-styling` before any change. Never edit `design.md` or a CSS/theme file directly, and never translate design intent into CSS yourself: `updating-styling` dispatches a `coding` sub-agent to run the project's `*-ux-theme-sync` skill, and the request is not complete until that sub-agent reports back. This is separate from ordinary component styling, which follows the rules above.

### File Safety

- Read all provided files and understand how they work together before editing.
- Only modify files explicitly allowed for modification.
- Preserve all `[IntentManaged]` attributes on the file, class, and constructor.
- Add all required `using` clauses in `.razor.cs` files and add `@using` directives in `.razor` files only when needed for Razor compilation or type resolution.
- Use existing services when available.

### Service Injection

- Use service proxies to communicate with other applications.
- If possible, validate the proxy and the other application have the same serialization settings.

### WASM Specific Guidance

- If you introduce any REST service proxies to the Razor Component, You MUST feedback to the orchestrating agent that it should validate the CORS setup app hosting the services.

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
- Render a button for each method whose name begins with one of these prefixes when you can confirm the method exists on the backing class: `NavigateTo*`, `Add*`, `Create*`, `New*`, `Edit*`, `Update*`, `Delete*`, `Remove*`, `View*`, `Open*`, `Search*`, `Load*`.
- If the method does not exist or intent is unclear, omit the button.
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
- When using NavigationManager.NavigateTo, ensure that the target URL is in the correct format. e.g. $"/customers/{CustomerId:guid}" is invalid and should be $"/customers/{CustomerId:D}" or the type can be omitted completely.

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

- Use `Icon=` (not `StartIcon=`) on `MudChip` — `StartIcon` was removed from `MudChip` in MudBlazor v7 and produces a MUD0002 compiler warning.
- Use `StartIcon=` (not `Icon=`) on `MudButton` — `Icon` is not a valid attribute on `MudButton`; only `MudChip` and `MudIcon`/`MudIconButton` use `Icon=`.
- Use `Justify=` (not `JustifyContent=`) on `MudStack` — `JustifyContent` was removed in MudBlazor v7 and produces a MUD0002 compiler warning.

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

- If a matching navigation method exists in the backing class, bind it with `OnClick`; otherwise use appropriate Blazor navigation markup.
- Include icon and display text when the design pattern supports them.
- Do not modify existing navigation methods.
- If a navigation item points to an Add page and the backing class already has a matching action method, create the page button from the method, not from the navigation item.

## App menu hand-off

How global navigation is modeled is covered by the `ui-modeling-blazor` instructions. This section covers only the hand-off that a `MainLayout` navigation change triggers.

- Never run the `blazor-app-menu` skill yourself and never edit menu files (`MainLayout*.razor` or the Profile menu file) directly — that work belongs to a `coding` sub-agent.
- When you create, delete, rename or change any `Navigation` association whose source is `MainLayout`, gather the context below and hand it to a `coding` sub-agent in a dedicated task, instructing it to call `use_skill` for `blazor-app-menu` before making any change and passing the context verbatim so it need not re-derive it. Do this ONCE, after all pages for the unit of work are implemented — never per page. The work is not complete until that sub-agent has reported back.
- For every `Navigation Target End` currently modeled on `MainLayout`: the target page's label and route, any required roles/policies, and the association's own Comment (the item's verbatim placement text).
- Whether the project is an Auth or NoAuth project and its hosting model — inspect `Program.cs`, or `Startup.cs` where the project has one, for `AddAuthorizationCore()`/`AddAuthorization()` and `AddCascadingAuthenticationState()`; check both before concluding NoAuth, and check whether a separate `.Client` project exists.
- Whether the Layout has theming enabled or disabled — the `coding` sub-agent needs this to decide whether `MainLayoutHeader.razor` should carry a `<ThemeToggle />`. `MainLayout.razor`'s attached AI context states it; when it doesn't, read it off the project: `MudThemeProvider` bound to `@IsDarkMode`, or a `ThemeToggle` component in the project, means enabled, while a hardcoded `IsDarkMode="true"` with no `ThemeToggle` means disabled.

## Validation Checklist

- [ ] All bindings and event handlers used in `.razor` exist in `.razor.cs`, and the code compiles
- [ ] No `@code` blocks were added to `.razor` files, and no comments were added
- [ ] `[IntentManaged]` attributes are preserved
- [ ] Existing global styles and theme values were not changed; component styles stay minimal and component-specific
- [ ] Forms are validated for create, save and update flows
- [ ] Any styling, theme, branding, typography or `design.md` request went through `updating-styling`
- [ ] Any `MainLayout` navigation change was handed to a `coding` sub-agent to reconcile the menu
