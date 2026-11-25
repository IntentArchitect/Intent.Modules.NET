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

          # Add-entity dialog (Bootstrap)

          For add or create dialogs. Not for pages or edit dialogs. A dialog closes through its `OnClosed` callback, never by navigating.

          ## 1. Read first

          1. `add-entity-dialog-sample.razor` and `add-entity-dialog-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the models, enums, lookups and services they use.
          3. `wwwroot/ux-tokens.css`, for the real token names. Missing it doesn't block.

          ## 2. Classify the `.razor.cs`

          - **Skeleton** (injections, model classes, empty or stub methods): add what the dialog needs, following the sample's `.razor.cs`: the `[Parameter] EventCallback<bool> OnClosed`, the model property initialised to a new instance, a saving flag, lookup loading in `OnInitializedAsync` when the model needs lookups and a service exists, `Save`/`SaveAsync` and `Cancel`.
          - **Implemented** (has service calls or save logic): wire the markup to the existing members and leave them, and the model's properties, exactly as they are.

          ## 3. Dialog

          - Markup: copy the sample's Bootstrap modal (`modal d-block` overlay, `modal-dialog`, `modal-content` with `modal-header`, `modal-body` and `modal-footer`), including its `btn-icon` close button calling `Cancel`. Never MudBlazor dialog APIs (`IMudDialogInstance`, `MudDialog.Close`, `DialogActions`): the project has no MudBlazor.
          - Save validates the form, calls the existing create method, invokes `OnClosed.InvokeAsync(true)` on success, and on error stays open and sets the existing error state, such as `ErrorMessage`.
          - Cancel only invokes `OnClosed.InvokeAsync(false)`: no service calls, no model reset.
          - Inputs arrive through `[Parameter]` properties or the project's existing conventions.

          ## 4. Form

          Use the sample's form pattern (`EditForm` with `DataAnnotationsValidator`) and bind only to members that exist on the model.

          | Property | Control |
          |---|---|
          | string | `InputText class="form-control"` |
          | bool | `InputCheckbox` inside a `form-check form-switch` wrapper |
          | enum | `InputSelect` with `<option value="@EnumType.Member">` entries, no numeric casting, from the enum's real definition, never the sample's members |
          | lookup | `InputSelect` over options loaded from an existing service |
          | date | `InputDate`, or `<input type="date">` bound with `@bind` for a field that isn't validated |
          | collection | a `<table>` row or a bordered block per item |

          - Required fields carry validation attributes (or the existing validator wiring) and show a `ValidationMessage`; an invalid form never reaches the service. The sample's footer Save sits outside the `EditForm`, so its submit skips validation: don't copy that; build the form on an `EditContext` and have the save method return early when `Validate()` is false.
          - Footer actions: Save is `btn btn-primary`, disabled while saving and showing a `spinner-border spinner-border-sm` meanwhile; Cancel is `btn btn-outline`.
          - Collection add and remove buttons only when matching methods exist. In a `for` loop, declare `var index = i;` first and use `index`, never `i`.

          ## 5. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Navigation in the dialog flow.
          - Changing existing methods, payload shape or model properties; inventing services, enum members or option data.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

          ## Definition of Done

          - [ ] The sample's Bootstrap modal structure, closed through `OnClosed`; no MudBlazor dialog APIs
          - [ ] Every binding resolves to a `.razor.cs` member; no `@code`, no service calls in `.razor`
          - [ ] Save validates, creates through the existing method and closes with `true`; Cancel only closes with `false`
          - [ ] Invalid input never reaches the service; required fields show a `ValidationMessage`; Save is disabled while saving
          - [ ] Enum options are verified `@EnumType.Member` values; lookups and services are real
          - [ ] Implemented code-behind: existing members and model properties unchanged
          - [ ] Styles use tokens; shared styles unchanged
          - [ ] The project builds; `.razor` has an `@using` for every type it names

          """);
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
