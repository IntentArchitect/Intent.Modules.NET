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

          # Edit-entity dialog (Bootstrap)

          For edit or update dialogs. Not for pages or add dialogs. A dialog closes through its `OnClosed` callback, never by navigating.

          ## 1. Read first

          1. `edit-entity-dialog-sample.razor` and `edit-entity-dialog-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the models, enums, lookups and services they use.
          3. `wwwroot/ux-tokens.css`, for the real token names. Missing it doesn't block.

          ## 2. Classify the `.razor.cs`

          - **Skeleton** (injections, model classes, empty or stub methods): add what the dialog needs, following the sample's `.razor.cs`: the `[Parameter] EventCallback<bool> OnClosed`, the input `[Parameter]` (an id, or a model to prepopulate from), the model property, a saving flag, loading the entity through the existing service in `OnInitializedAsync`, and the save and cancel methods (the sample's `OnSubmit` and `OnCancel`).
          - **Implemented** (has loading, service calls or save logic): wire the markup to the existing members and leave them, and the model's properties, exactly as they are.

          ## 3. Dialog

          - Markup: copy the sample's Bootstrap modal (`modal d-block` overlay, `modal-dialog`, `modal-content` with `modal-header`, `modal-body` and `modal-footer`), including its `btn-icon` close button calling the cancel method. While the model is `null`, the body shows only the sample's centred `spinner-border`. Never MudBlazor dialog APIs (`IMudDialogInstance`, `MudDialog.Close`, `DialogActions`): the project has no MudBlazor.
          - Save validates the form, calls the existing update method, invokes `OnClosed.InvokeAsync(true)` on success, and on error stays open and sets the existing error state, such as `ErrorMessage`.
          - Cancel only invokes `OnClosed.InvokeAsync(false)`: no service calls, no model reset.

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
          - Footer actions: Cancel is `btn btn-outline`; Save is `btn btn-primary`, disabled while saving or while the model is still loading.
          - Collection add and remove buttons only when matching methods exist. In a `for` loop, declare `var index = i;` first and use `index`, never `i`.

          ## 5. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Navigation in the dialog flow.
          - Changing existing methods, payload shape or model properties; inventing services, enum members or option data.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

          ## Definition of Done

          - [ ] The sample's Bootstrap modal structure, closed through `OnClosed`; no MudBlazor dialog APIs; spinner while loading
          - [ ] Every binding resolves to a `.razor.cs` member; no `@code`, no service calls in `.razor`
          - [ ] The entity loads (or prepopulates) through existing methods; Save validates, updates through the existing method and closes with `true`; Cancel only closes with `false`
          - [ ] Invalid input never reaches the service; required fields show a `ValidationMessage`; Save is disabled while saving or loading
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
