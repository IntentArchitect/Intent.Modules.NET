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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorDialogEditingEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorDialogEditingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorDialogEditingEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorDialogEditingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-dialog-editing-entity")
        .FromMarkdown("""
          ---
          name: blazor-dialog-editing-entity
          description: Implements Blazor edit or update entity dialogs using Bootstrap modal dialog patterns and valid form submission, preserving existing .razor.cs loading and service behavior while wiring save and cancel correctly through the dialog's OnClosed callback. Use when creating or implementing edit or update entity dialogs in Blazor, including when an empty or skeleton dialog already exists and needs its razor markup or code-behind filled in.
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
          1. `edit-entity-dialog-sample.razor`
          2. `edit-entity-dialog-sample.razor.cs`

          **Target component and project files:**
          3. The target `.razor` and `.razor.cs`
          4. Related project files: models, enums, lookups, services

          **Design and styling context** (search the project — these are NOT in the SKILL.md folder):
          5. `design.md` — search for this file anywhere in the project; read it in full if found; if absent, note the absence and continue without design context
          6. `ux-tokens.css`, `ux-base.css`, `ux-components.css` — read from the project's `wwwroot` folder if present; note any that are absent

          If any sample file (items 1–2) cannot be accessed: stop immediately, confirm the SKILL.md folder location, retry from that location, and if still inaccessible report which file is missing. Do not proceed with partial implementation or approximation.
          If items 5–6 are not found: note the absence and continue — they are reference context, not blocking.

          ---

          ## Assess The .razor.cs Before Writing

          Use for: Edit or update entity dialogs in Blazor
          Do NOT use for: Full pages, search pages, add dialogs, or non-Blazor projects
          This is a dialog: close through the dialog's `OnClosed` callback rather than navigation

          Read the existing `.razor.cs` in full. Determine whether it is a **skeleton** (constructor, injections, and empty or stub methods only) or **implemented** (contains real data loading, model construction, or service calls).

          **If skeleton** — scaffold the missing members modelled on the sample `.razor.cs`:
          - Add `[Parameter]` properties needed for the dialog (e.g. entity ID)
          - Add a model field or property matching the sample pattern
          - Implement `OnInitializedAsync()` or `OnParametersSetAsync()` to load the entity via the appropriate service (search the project for a matching service interface)
          - Add `Save()` / `SaveAsync()` that validates the form, calls the existing update service method, and invokes `OnClosed.InvokeAsync(true)` on success
          - Add `Cancel()` that only invokes `OnClosed.InvokeAsync(false)`
          - Add supporting methods only when they exist in the sample and the relevant service methods exist in the project

          **If implemented** — preserve all existing logic exactly:
          - Do NOT modify existing methods, service calls, or payload construction
          - Do NOT add, rename, or remove model properties
          - Do NOT rewrite existing C# functionality

          **Always forbidden** (skeleton or implemented):
          - Inventing service classes or interfaces that don't exist in the project
          - Calling services directly from the `.razor` file
          - Adding navigation logic to the dialog flow
          - Putting C# logic in `.razor` using `@code`

          ---

          ## 1. Dialog Structure And Data Loading

          This component is a Bootstrap modal dialog, not a page.

          Dialog rules:
          - In `.razor.cs`, expose a `[Parameter] EventCallback<bool> OnClosed` so the caller can react to close/save
          - In `.razor`, use the Bootstrap modal structure: an outer `<div class="modal d-block" tabindex="-1" style="background-color: rgba(0,0,0,.5)">` wrapping `<div class="modal-dialog">`/`<div class="modal-content">`, with `modal-header`, `modal-body`, and `modal-footer` sections
          - While the entity is loading, render a centered `spinner-border` in the modal body instead of the form, matching the sample
          - Do not use MudBlazor dialog APIs (`IMudDialogInstance`, `MudDialog.Close`/`MudDialog.Cancel`, `TitleContent`/`DialogContent`/`DialogActions`) — this project has no MudBlazor dependency
          - For success, invoke `OnClosed.InvokeAsync(true)`
          - For cancel, invoke `OnClosed.InvokeAsync(false)`

          Data loading:
          - Receive dialog input through `[Parameter]` properties — add them if absent in a skeleton
          - If an ID is passed, load the entity via the appropriate service method (implement `OnInitializedAsync()` if it is an empty stub)
          - If a model is passed, prepopulate from that input structure

          ---

          ## 2. Save And Cancel Methods

          `Save()` or `SaveAsync()` — add if absent in a skeleton:
          1. Validate the form
          2. Call the existing update service method without modification
          3. On success, invoke `OnClosed.InvokeAsync(true)`
          4. On error, keep the dialog open and set existing error state such as `ErrorMessage`

          `Cancel()` — add if absent in a skeleton:
          - Only invoke `OnClosed.InvokeAsync(false)`
          - Do not reset model state
          - Do not call services

          Template bindings:
          - Bind Save button to `Save()` or `SaveAsync()`
          - Bind Cancel button to `Cancel()`
          - Do not call backend methods directly from the Razor template

          ---

          ## 3. Form Validation

          Use valid Blazor `EditForm` patterns with the project's existing validation approach.

          Required fields must have:
          - Existing validation annotations or validator wiring
          - Visible validation messages
          - No service call when invalid

          Save button state:
          - Disable Save when the form is invalid
          - Disable Save while saving

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

          Rules:
          - Bind directly to the enum member as the `<option value="@EnumType.Member">` value — no numeric casting needed
          - Never assume enum members from sample code without verifying against the real enum definition

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
          - Save button must use the `btn btn-primary` class with `disabled` bound to the saving flag

          **Design and styling context**

          Apply the design token and CSS utility context from the files you read in the mandatory phase. Use `var(--token)` for all inline `Style=` attributes — never hardcode hex values. Verify utility classes (e.g. `ux-fade-in-up`, `ux-gradient-primary`) exist before applying. The design context informs styling choices only — it does not override layout structure.

          ---

          ## Definition of Done

          - [ ] All bindings used in `.razor` resolve to members in `.razor.cs` (including any members just scaffolded)
          - [ ] No `@code` block was introduced in `.razor`
          - [ ] Dialog uses the Bootstrap modal structure and an `OnClosed` callback, not MudBlazor dialog APIs
          - [ ] Entity data is loaded or prepopulated through lifecycle or backing methods (implemented or added if the skeleton had none)
          - [ ] Save invokes `OnClosed.InvokeAsync(true)` on success
          - [ ] Cancel only invokes `OnClosed.InvokeAsync(false)`
          - [ ] Existing update service methods were not modified
          - [ ] Model properties were not arbitrarily renamed or removed — additions are allowed only when scaffolding a skeleton
          - [ ] No service classes or interfaces were invented that don't exist in the project
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
