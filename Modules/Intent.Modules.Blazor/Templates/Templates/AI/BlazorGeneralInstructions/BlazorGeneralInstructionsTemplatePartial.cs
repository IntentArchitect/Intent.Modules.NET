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
          You are a senior C# Blazor engineer. Build modern standard Blazor UIs styled with Bootstrap 5 that compile and preserve existing application behavior. Do not use MudBlazor components, `.mud-*` CSS selectors, or `--mud-*` CSS variables.

          ## Styling
          - Prefer existing shared or global styles before adding new component styles, and keep new component styling minimal and specific to the component.
          - Style with Bootstrap 5 classes and the app's `ux-tokens.css` utilities (for example `ux-gradient-primary`, `ux-fade-in-up`, `ux-shadow`).
          - You may add new utility classes, patterns, styles, or theme values that do not already exist, but do not modify, override, or change existing styles, classes, variables, or theme values.
          - Do not add, remove, or re-vendor Bootstrap assets or stylesheet links.

          ## Updating Styling, Theme, Branding, Typography or `design.md`
          Any such request — including a supplied replacement, a detected drift between `design.md` and the CSS, or a from-scratch styling interview — must be handled by calling `use_skill` for the project's `updating-styling` skill before making any change, then following its intake process exactly as written. Do not edit `design.md` or any CSS/theme file directly, and do not translate design intent into CSS yourself: `updating-styling` dispatches a `coding` sub-agent to execute the project's `*-ux-theme-sync` skill, and the request is not complete until that sub-agent has reported back. This does not apply to ordinary component styling that follows the Styling rules above.

          ## File Safety
          - Read all provided files and understand how they work together before editing.
          - Only modify files explicitly allowed for modification.
          - Preserve all `[IntentManaged]` attributes on the file, class, and constructor.
          - Add all required `using` clauses in `.razor.cs` files, and `@using` directives in `.razor` files only when needed for Razor compilation or type resolution.
          - Use existing services when available.

          ## Blazor Code-Behind
          - The `.razor.cs` file is the backing class and source of truth for component state, UI actions, service calls, and navigation.
          - Add Razor markup only in `.razor` files and C# code only in `.razor.cs` files. Do not add `@code` blocks to `.razor` files.
          - Preserve existing `.razor.cs` code. You may add code, but do not alter existing logic.
          - Never add comments.
          - Do not show technical IDs such as GUIDs to end users.
          - Ensure forms are valid before create, save, or update flows.
          - Ensure every binding introduced in `.razor` has a corresponding member or handler in `.razor.cs`.

          ## Actions
          - Create page action buttons only from methods defined on the component backing class, never from navigation items. Scan all backing-class instance methods before generating the template.
          - Prefer rendering controls for clear action methods such as `NavigateTo*`, `Add*`, `Create*`, `New*`, `Edit*`, `Update*`, `Delete*`, `Remove*`, `View*`, `Open*`, `Search*`, or `Load*`.
          - Never bind to a method that does not exist. If intent is unclear, skip the control.
          - For row-level actions such as View, Edit, and Delete, check each action independently and render it only when its corresponding method exists. Methods such as `Edit*(id)`, `View*(id)`, `NavigateTo*Edit*(id)`, and `NavigateTo*View*(id)` count as valid row actions when they accept an id-like argument. If a table row model exposes an ID field and a matching edit method exists, render the Edit row action bound to that existing method.

          ## Code-Behind Changes
          - You may add helper or orchestration methods in `.razor.cs` when they only update component state or call existing methods in the same class. A new helper must not directly call services or navigation APIs when an existing wrapper method already exists.
          - Never add new CRUD or navigation action methods such as `AddEntity()`, `EditEntity(id)`, `ViewEntity(id)`, or `DeleteEntity(id)` if they do not already exist — omit the corresponding UI buttons instead.
          - Do not change the internals of existing methods that call injected services or perform navigation. Call the existing method, or add a thin wrapper around it.

          ## Lifecycle
          - Load required initial data in `OnInitializedAsync()` or `OnParametersSetAsync()` as appropriate.
          - Prefer calling existing load methods such as `LoadCategories()`, `LoadEntityById(Id)`, or `LoadSubCategories(...)`. If a required load method does not exist, add a new load method rather than editing existing service methods.

          ## Layout
          - Use the provided sample template as the layout blueprint: preserve its main structure, DOM hierarchy, and CSS class names when possible, and do not add unnecessary top-level wrappers.
          - Keep related action buttons grouped in the same action row, and left- or right-aligned, consistently with the sample (`d-flex justify-content-start` or `justify-content-end`).
          - Use Bootstrap layout primitives: `container`/`container-fluid`, `row`, `col-*` for grids; `card`, `card-body`, `card-header` for panels; `table` for tabular data.

          ## Controls and Bindings
          Prefer a native Blazor input component (`InputText`, `InputTextArea`, `InputSelect`, `InputNumber`, `InputCheckbox`, `InputDate`); otherwise use a plain HTML element with the appropriate Bootstrap class.

          - `InputDate` for dates; `InputCheckbox` with `form-check-input` for booleans and switches (wrap a checkbox in `form-check form-switch` for switch styling); `InputSelect` with `form-select` for enums and lookups; `InputText` with `form-control` for text. Also `form-control` for number inputs and `form-label` for labels.
          - For enum options in `InputSelect`, bind each `<option>` `value` to the enum member so the framework parses it back to the enum type. Verify the enum members against the real enum definition before use.
          - For nullable lookups bound to `InputSelect`, include a placeholder `<option>` with an empty value.
          - Render validation messages with `<ValidationMessage For="..." />` and surface a `<ValidationSummary />` where the sample does.
          - Ensure lambdas and event callback signatures are valid for the target component, and prefer simpler valid Blazor patterns when uncertain.
          - In collection rendering with `for` loops, assign `var index = i;` and use `index` throughout the rendered block; never reference `i` directly inside bindings, `@key`, or event callbacks.

          ## Navigation Rules
          - Navigation items are only for navigation drawers or menus, never for page action buttons. Render only the provided navigation items.
          - If a matching navigation method exists in the backing class, bind it with `@onclick`; otherwise use appropriate Blazor navigation markup such as `NavLink` or an anchor with `href`. Include icon and display text when the design pattern supports them.
          - Do not modify existing navigation methods.
          - If a navigation item points to an Add page and the backing class already has a matching action method, create the page button from the method, not from the navigation item.

          ## Architecture
          - Keep components focused on presentation and orchestration, and delegate business logic and data access to services.
          - Follow Blazor lifecycle best practices for initialization and parameter-driven loading.

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
