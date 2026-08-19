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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorDialogAddingEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorDialogAddingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorDialogAddingEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorDialogAddingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-dialog-adding-entity")
        .FromMarkdown("""
          ---
          name: blazor-dialog-adding-entity
          description: Implements Blazor add or create entity dialogs using Bootstrap modal dialog patterns and valid form submission, preserving existing .razor.cs service behavior while wiring save and cancel correctly through the dialog's OnClosed callback. Use when creating or implementing add or create entity dialogs in Blazor, including when an empty or skeleton dialog already exists and needs its razor markup or code-behind filled in.
          paths:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/design.md"
          - "**/ux-tokens.css"
          - "**/ux-base.css"
          - "**/ux-components.css"
          ---

          ## MANDATORY: Read Samples Before Implementation

          STOP — you MUST read ALL of the following before writing ANY code:

          **Samples** (in the SAME folder as this SKILL.md):
          1. `add-entity-dialog-sample.razor`
          2. `add-entity-dialog-sample.razor.cs`

          **Target component and project files:**
          3. The target `.razor` and `.razor.cs`
          4. Related project files: models, enums, lookups, services

          **Design and styling context** (search the project — these are NOT in the SKILL.md folder):
          5. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
          6. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

          If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
          If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

          ---

          ## Preserve Existing Implementation

          Use for: Add or create entity dialogs in Blazor
          Do NOT use for: Full pages, search pages, edit dialogs, or non-Blazor projects
          Source of truth: Existing `.razor.cs` file defines service calls, dialog behavior, and model structure
          This is a dialog: close through the dialog's `OnClosed` callback rather than navigation

          ### You MUST NOT:
          - Modify existing backend methods such as `CreateEntity()` or `CreateEntityAsync()`
          - Change payload shape sent to the backend
          - Add, rename, or remove model properties
          - Invent lookup services
          - Rewrite existing C# functionality
          - Add page navigation logic to the dialog flow
          - Put C# logic in `.razor` using `@code`

          ---

          ## Assess The .razor.cs Before Writing

          Read the existing `.razor.cs` in full. Determine whether it is a **skeleton** (constructor, injections, and empty or stub methods only) or **implemented** (contains real model construction, service calls, or save logic).

          **If skeleton** — scaffold the missing members modelled on the sample `.razor.cs`:
          - Add a model field or property (initialized to a new instance) matching the sample pattern
          - Add `Save()` / `SaveAsync()` that validates the form, calls the existing create service method, and invokes `OnClosed.InvokeAsync(true)` on success
          - Add `Cancel()` that only invokes `OnClosed.InvokeAsync(false)`
          - Add lookup loading in `OnInitializedAsync()` only if lookups are required by the model and a matching service exists in the project

          **If implemented** — preserve all existing logic exactly:
          - Do NOT modify existing methods, service calls, or payload construction
          - Do NOT add, rename, or remove model properties
          - Do NOT rewrite existing C# functionality

          ---

          ## 1. Dialog Structure

          This component is a Bootstrap modal dialog, not a page.

          Dialog rules:
          - In `.razor.cs`, expose a `[Parameter] EventCallback<bool> OnClosed` so the caller can react to close/save
          - In `.razor`, use the Bootstrap modal structure: an outer `<div class="modal d-block" tabindex="-1" style="background-color: rgba(0,0,0,.5)">` wrapping `<div class="modal-dialog">`/`<div class="modal-content">`, with `modal-header`, `modal-body`, and `modal-footer` sections
          - The header's close control is a `<button type="button" class="btn-icon" aria-label="Close" @onclick="Cancel">&times;</button>`
          - Do not use MudBlazor dialog APIs (`IMudDialogInstance`, `MudDialog.Close`/`MudDialog.Cancel`, `TitleContent`/`DialogContent`/`DialogActions`) — this project has no MudBlazor dependency
          - For success, invoke `OnClosed.InvokeAsync(true)`
          - For cancel, invoke `OnClosed.InvokeAsync(false)`

          If input data is needed, receive it through `[Parameter]` properties or existing project conventions.

          ---

          ## 2. Save And Cancel Methods

          Implement or use top-level methods for the template:

          `Save()` or `SaveAsync()`:
          1. Validate the form
          2. Call the existing backend method without modifying it
          3. On success, invoke `OnClosed.InvokeAsync(true)`
          4. On error, keep the dialog open and set existing error state such as `ErrorMessage`

          `Cancel()`:
          - Only invoke `OnClosed.InvokeAsync(false)`
          - Do not reset model state
          - Do not call services

          Template bindings:
          - Bind Save button to `Save()` or `SaveAsync()`
          - Bind Cancel button to `Cancel()`
          - Do not call service methods directly from the Razor template

          ---

          ## 3. Form And Validation In Dialogs

          Use valid Blazor form patterns such as `EditForm` with the component's existing validation conventions.

          Required fields must have:
          - Existing validation annotations or validator wiring
          - Visible validation messages
          - No service call when invalid

          Save button state:
          - Disable Save when the form is invalid
          - Disable Save while a save is in progress

          ---

          ## 4. Control Mapping

          | Property Type | Control |
          |---------------|---------|
          | String | `InputText` (`class="form-control"`) |
          | Boolean | `InputCheckbox` rendered inside a `form-check form-switch` wrapper |
          | Enum | `InputSelect` with verified members as literal `<option value="@EnumType.Member">` entries |
          | Lookup | `InputSelect` from real option sources only |
          | Array | Repeatable Bootstrap blocks (a `<table>` row per item, or a bordered `<div>` block) |
          | Date | `InputDate`, or a plain `<input type="date">` bound with `@bind` for non-validated fields |

          Enum rules:
          - Read and verify the enum definition before rendering options
          - Use only verified members
          - Bind directly to the enum member as the `<option value="@EnumType.Member">` value — no numeric casting needed
          - Never assume enum members from the sample files

          ---

          ## 5. Child Collections

          - Add collection buttons only when matching backing methods exist
          - Remove collection buttons only when matching backing methods exist
          - In `for` loops, use `var index = i;` and never bind directly to `i`

          ---

          ## 6. Styling

          - Prefer shared utilities first
          - Keep component-specific styles minimal
          - Never modify existing shared styles or theme values
          - Match the sample dialog layout closely
          - Cancel button in the modal footer must use the `btn btn-outline` class
          - Save button must use the `btn btn-primary` class with `disabled` bound to the saving flag, showing a `spinner-border spinner-border-sm` while saving

          **Design and styling context**

          Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

          ---

          ## Definition of Done

          - [ ] All bindings used in `.razor` exist in `.razor.cs` (including any members just scaffolded)
          - [ ] No `@code` block was introduced in `.razor`
          - [ ] Dialog uses the Bootstrap modal structure and an `OnClosed` callback, not MudBlazor dialog APIs
          - [ ] Save invokes `OnClosed.InvokeAsync(true)` on success
          - [ ] Cancel only invokes `OnClosed.InvokeAsync(false)`
          - [ ] Service methods were not called directly from the template
          - [ ] Backend methods were not modified
          - [ ] Model properties were not added, removed, or renamed
          - [ ] Enum options were verified against the real enum definition
          - [ ] Validation prevents service calls when invalid
          - [ ] Shared styles were preserved and component styling remained minimal

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
