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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorGeneralInstructions
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorGeneralInstructionsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorGeneralInstructionsTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorGeneralInstructionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"blazor-general.instructions")
        .FromMarkdown("""
          ---
          description: Instructions for implementing standard Blazor components with Bootstrap 5 and modern UI best practices, without a component library such as MudBlazor.
          appliesTo:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/*.razor.css"
          ---

          ## Role and Context
          You are a senior C# Blazor engineer. Build modern standard Blazor UIs styled with Bootstrap 5 that compile, follow best practices, and preserve existing application behavior. Do not use MudBlazor components, MudBlazor CSS selectors, or `--mud-*` CSS variables.

          ## Core Rules

          ### Styling
          - Prefer existing shared or global styles before adding new component styles.
          - Style with Bootstrap 5 classes and the app's `ux-tokens.css` utilities (for example `ux-gradient-primary`, `ux-fade-in-up`, `ux-shadow`).
          - Keep component styling minimal and specific to the component.
          - You may add new utility classes, patterns, styles, or theme values when they do not already exist.
          - Do not modify, override, or change existing styles, classes, variables, or theme values.
          - Do not add, remove, or re-vendor Bootstrap assets or stylesheet links.
          - Any request to update styling, the theme, colours/branding, typography, or `design.md` itself is out of scope to edit directly — see "Updating Styling" below; it must go through the `updating-styling` skill and a `coding` sub-agent.

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
          - Use Bootstrap layout primitives: `container`/`container-fluid`, `row`, `col-*` for grids; `card`, `card-body`, `card-header` for panels; `table` for tabular data.
          - Keep action buttons left-aligned or right-aligned consistently with the sample (`d-flex justify-content-start` or `justify-content-end`).

          ### Control Selection
          1. Native Blazor input component (`InputText`, `InputTextArea`, `InputSelect`, `InputNumber`, `InputCheckbox`, `InputDate`)
          2. Plain HTML element with the appropriate Bootstrap class

          Use `InputDate` for dates, `InputCheckbox` with `form-check-input` for booleans, `InputSelect` with `form-select` for enums and lookups, and `InputText` with `form-control` for text where appropriate.

          ### Bootstrap Binding Rules
          - Apply the correct Bootstrap class to every control: `form-control` for text and number inputs, `form-select` for selects, `form-check-input` for checkboxes and switches, `form-label` for labels.
          - For switch styling, wrap a checkbox in `form-check form-switch`.
          - For enum options in `InputSelect`, bind each `<option>` `value` to the enum member so the framework parses it back to the enum type. Verify the enum members against the real enum definition before use.
          - For nullable lookups bound to `InputSelect`, include a placeholder `<option>` with an empty value.
          - Render validation messages with `<ValidationMessage For="..." />` and surface a `<ValidationSummary />` where the sample does.

          ### Template Safety
          - Ensure all bindings between `.razor` and `.razor.cs` are valid and the code compiles.
          - Ensure lambdas and event callback signatures are valid for the target component.
          - Prefer simpler valid Blazor patterns when uncertain.
          - In collection rendering with `for` loops, do not reference `i` directly inside bindings, `@key`, or event callbacks.
          - In `for` loops, assign `var index = i;` and use `index` throughout the rendered block.

          ## Navigation Rules
          - Navigation items are only for navigation drawers or menus, never for page action buttons.
          - Render only the provided navigation items.
          - If a matching navigation method exists in the backing class, bind it with `@onclick`; otherwise use appropriate Blazor navigation markup such as `NavLink` or an anchor with `href`.
          - Include icon and display text when the design pattern supports them.
          - Do not modify existing navigation methods.
          - If a navigation item points to an Add page and the backing class already has a matching action method, create the page button from the method, not from the navigation item.

          ## Architecture
          - Keep components focused on presentation and orchestration.
          - Delegate business logic and data access to services.
          - Follow Blazor lifecycle best practices for initialization and parameter-driven loading.
          - Keep Razor templates and code-behind implementations aligned so bindings remain valid and maintainable.

          ## Updating Styling

          > **Scope guard:** This section applies whenever you are asked to update styling, the theme, colours/branding, typography, or `design.md` itself — not to ordinary component styling that already follows the Styling rules above.

          > **Trigger (must hand off):** Any request to change the palette, typography, theme, or `design.md` — including a supplied replacement, a detected drift between `design.md` and the CSS, or a from-scratch styling interview — MUST be handled by calling `use_skill` for the project's `updating-styling` skill before making any change. Do not edit `design.md` or any CSS/theme file directly, and do not translate design intent into CSS yourself.

          - Call `use_skill` for `updating-styling` first and follow its intake process exactly as written.
          - `updating-styling` itself dispatches a `coding` sub-agent to execute the project's `*-ux-theme-sync` skill — never perform that CSS/token translation yourself, and never skip or shortcut that hand-off.
          - Do not consider a styling/theme/design.md request complete until `updating-styling` has run its full intake-to-dispatch flow and the `coding` sub-agent it dispatched has reported back.

          ## Validation Checklist
          - [ ] If styling, the theme, colours/branding, typography, or `design.md` was requested, the `updating-styling` skill was used and a `coding` sub-agent was dispatched to run the `*-ux-theme-sync` skill.
          - [ ] All bindings and event handlers used in `.razor` exist in `.razor.cs`.
          - [ ] No `@code` blocks were added to `.razor` files.
          - [ ] `[IntentManaged]` attributes are preserved.
          - [ ] Required `using` directives were added and the code compiles.
          - [ ] No comments were added.
          - [ ] No MudBlazor components, `.mud-*` selectors, or `--mud-*` variables were introduced.
          - [ ] No Bootstrap assets or stylesheet links were added, removed, or edited.
          - [ ] Existing global styles and theme values were not changed.
          - [ ] Component styles remain minimal and component-specific.
          - [ ] Forms are validated for create, save, and update flows.

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
