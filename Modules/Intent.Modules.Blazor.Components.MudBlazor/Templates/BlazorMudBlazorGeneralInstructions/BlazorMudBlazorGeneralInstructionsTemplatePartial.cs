using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.BlazorMudBlazorGeneralInstructions
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorMudBlazorGeneralInstructionsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Components.MudBlazor.BlazorMudBlazorGeneralInstructionsTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorMudBlazorGeneralInstructionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"blazor-mudblazor-general.instructions")
        .FromMarkdown("""
          ---
          description: Instructions for implementing Blazor components with MudBlazor and modern UI best practices.
          appliesTo:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/*.razor.css"
          ---

          ## Role and Context
          You are a senior C# Blazor engineer. Build modern MudBlazor UIs that compile, follow best practices, and preserve existing application behavior.

          ## Core Rules



          ### Mandatory coding handoff

          - Any implementation work for this component must be delegated to a coding subagent.
          - You are not permitted to implement the code directly.

          #### Required instructions for the coding subagent
          Include these instructions in the coding subagent task:

          - Before any file inspection or code change, determine whether an applicable skill exists.
          - If an applicable skill exists, you must call `use_skill` for that skill before continuing.
          - Do not read, patch, write, or generate code until the skill has been loaded.
          - Only proceed without loading a skill if no applicable skill exists.
          - If no applicable skill exists, implement the code directly.

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

          ### Service Injection
          - When `IScopedMediator`, `ISender`, or `IMediator` is available in the project, prefer it over `HttpClient` for all service calls.
          - Use `HttpClient` only when the Blazor application is a standalone client project that calls a **separate** API over HTTP (i.e., the project contains no application-layer handlers or commands).
          - If MediatR command or query classes (e.g. `GetCustomersQuery`, `DeleteCustomerCommand`) exist anywhere in the solution, inject `IScopedMediator` and call `await Mediator.Send(new XxxQuery(...))` — do not construct HTTP request URIs manually.
          - Never mix the two patterns in the same component.

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

          ## Global Navigation Modeling

          > **Scope guard:** This section applies only when you are explicitly asked to model UI navigation in the Intent User Interface designer. Skip this section during component code generation.

          > **Trigger (must run `updating-app-menu`):** If you create, delete, rename, or change any `Navigation` association where the source is `MainLayout` (i.e. any change that creates/changes a `Navigation Target End` on `MainLayout`), you MUST run the `updating-app-menu` skill before finishing.

          ### Root-level entry pages

          Treat a page as a root-level entry page when:

          - Its route is stable and does not require route parameters (e.g. no `{id}`), and
          - It represents a top-level capability a user would reasonably access directly from the global application shell (typically list/search/dashboard pages).

          ### Required modeling steps for root entry pages

          For each root-level entry page, unless stated otherwise, you MUST:

          - Add a `Navigation` association from `MainLayout` to the page, which creates a `Navigation Target End` on `MainLayout` for that page.
          - The presence of a `Navigation Target End` on `MainLayout` is itself the signal that a menu item should be created for that page — no separate modeling of Navigation items is required.
          - State which region (Header/Sider/Footer/Profile) the menu item belongs in directly in the association's own Comment (e.g. "Navigate to the product list page from the sider menu"). There is no stereotype for this any more — the comment is the sole placement signal.
          - If the comment doesn't state a region, the menu item defaults to the **Sider** region.
          - State the region as **Profile** in the comment when the page should appear in the **Profile/account dropdown** menu.
          - After adding the `MainLayout → Page` `Navigation` association for a root page, queue `updating-app-menu` to run ONCE at the end of the unit of work.

          ### Non-root / workflow pages (create/edit/detail/manage)

          Non-root/workflow/subordinate pages include create/add, edit/update, details/view, manage pages, and any page requiring route parameters (e.g. `/products/{id}`).

          Default rule:

          - Do NOT add `MainLayout` navigations (and therefore do NOT create global menu items) for non-root/workflow/subordinate pages unless the user explicitly confirms they should be directly reachable from global navigation.

          If the user explicitly models it anyway:

          - If the user explicitly instructs that a non-root/workflow page must appear in global navigation (including **Sider**, **Header**, **Footer**, or **Profile**), you MUST model it by adding a `Navigation` association from `MainLayout` to that page and stating the required region in that association's Comment.
          - In all such cases, you MUST still run the `updating-app-menu` skill (the same as for any other `MainLayout` navigation change). The **Profile** region is not treated as special for triggering or processing menu updates.

          ### Ambiguity

          If it’s unclear which pages are root-level entry points or whether non-root/workflow pages should be directly reachable from global navigation, ask the user which screens they want exposed in the global navigation and which region they should appear in (Sider/Header/Footer/Profile).

          ### Updating the app menu

          - The `updating-app-menu` skill MUST ALWAYS be run once you (the main agent) have finished implementing all pages for the current unit of work — never per-page, and never immediately after a single modeling change.
          - You, the main agent, must run this skill yourself. Never delegate execution to a `coding` sub-agent — a sub agent may be instructed to implement the menu triggered from the skill, but you MUST execute the skill first.
          - Do not consider the task complete until the `updating-app-menu` skill has been called to determine if the menu structure should be updated.
          - **CRITICAL Enforcement:** If you add or modify any `Navigation` association from `MainLayout` (i.e., create/modify a `Navigation Target End`), you MUST run the `updating-app-menu` skill before marking the work complete, even if the page already existed or no code generation changes were detected.

          ## Validation Checklist
          - [ ] All bindings and event handlers used in `.razor` exist in `.razor.cs`.
          - [ ] No `@code` blocks were added to `.razor` files.
          - [ ] `[IntentManaged]` attributes are preserved.
          - [ ] Required `using` directives were added and the code compiles.
          - [ ] No comments were added.
          - [ ] Existing global styles and theme values were not changed.
          - [ ] Component styles remain minimal and component-specific.
          - [ ] Forms are validated for create, save, and update flows.
          - [ ] If a `Navigation Target End` was added to MainLayout, the `updating-app-menu` skill was run by the main agent (after all pages were implemented) to reconcile the menu

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
