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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageAddingEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorPageAddingEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageAddingEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorPageAddingEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-adding-entity")
        .FromMarkdown("""
          ---
          name: blazor-page-adding-entity
          description: Implements Blazor add or create entity pages using Bootstrap/EditForm forms, preserving existing .razor.cs service and navigation behavior while wiring a valid save flow and model-bound UI. Use when creating or implementing add, create, new, insert, or register entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
          paths:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/design.md"
          - "**/ux-tokens.css"
          - "**/ux-base.css"
          - "**/ux-components.css"
          ---

          # Add-entity page (Bootstrap)

          For add or create pages. Not for search, edit or view pages, or dialogs.

          ## 1. Read first

          1. `add-entity-sample.razor` and `add-entity-sample.razor.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the models, enums, lookups and services they use.
          3. If present: `design.md` (anywhere in the project) and `wwwroot/ux-tokens.css`, `ux-base.css`, `ux-components.css`. Missing ones don't block.

          ## 2. Classify the `.razor.cs`

          - **Skeleton** (injections, model classes, empty or stub methods): add what the page needs, following the sample's `.razor.cs`: the model property initialised to a new instance, a saving flag, lookup loading in `OnInitializedAsync` when the model needs lookups and a service exists, and a `Save`/`SaveAsync` that calls the existing create service method and navigates on success.
          - **Implemented** (has model construction, service calls or save logic): wire the markup to the existing members and leave them, and the model's properties, exactly as they are.

          ## 3. Layout

          Copy the sample's structure: hero header, then one main card holding the form, then right-aligned actions, with no extra wrappers. Keep it unless the user asks otherwise; `design.md` informs styling, not layout. Reuse the sample's utility classes (`ux-gradient-primary`, `ux-fade-in-up`, ...) when they exist: grep each name as a substring across `wwwroot/**/*.css`, since they are often compound selectors.

          ## 4. Form

          Use the sample's form pattern (`EditForm` with `OnValidSubmit` and `DataAnnotationsValidator`) and bind only to members that exist on the model.

          | Property | Control |
          |---|---|
          | string | `InputText` (`InputTextArea` for long text) |
          | bool | `InputCheckbox` inside a `form-check form-switch` wrapper |
          | enum | `InputSelect` with `<option value="@EnumType.Member">` entries, no numeric casting, from the enum's real definition, never the sample's members |
          | lookup | `InputSelect` over options loaded from an existing service |
          | date | `InputDate`, or `<input type="date">` bound with `@bind` |
          | collection | a repeated Bootstrap block per item, keyed with `@key` |

          - With `ValueChanged` on an `InputSelect`, also set `Value` and `ValueExpression`, not `@bind-Value`.
          - A nullable nested object goes behind a toggle, only when the model supports it: off sets it to `null`, on creates it if `null`, and its fields show only while it is on.
          - Required fields carry validation attributes (or the existing validator wiring) and show a `ValidationMessage` when invalid.
          - Save is a submit button, so `OnValidSubmit` keeps invalid input from the service; it is disabled while a save is running.

          ## 5. Collections

          - In a skeleton, add `AddX` / `RemoveX` methods for the model's collections, as the sample does. In an implemented code-behind, show an add or remove button only when the matching method already exists.
          - In a `for` loop, declare `var index = i;` first and use `index`, never `i`, for bindings, `@key` and callbacks.

          ## 6. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Changing existing service calls or payload construction.
          - Inventing services, interfaces, enum members or option data.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; add a shared utility only for a reusable pattern that is missing, and keep component CSS minimal.

          ## Definition of Done

          - [ ] Hero header + main card as in the sample, with its utility classes where they exist
          - [ ] Every binding resolves to a `.razor.cs` member; no `@code`, no service calls in `.razor`
          - [ ] Save submits through `OnValidSubmit`, creates through the existing service method, and navigates on success
          - [ ] Save is disabled while saving; required fields show a `ValidationMessage`
          - [ ] Enum options are verified `@EnumType.Member` values; lookups and services are real
          - [ ] Collection buttons only with backing methods; item blocks keyed with `@key`; loops use `var index = i`
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
