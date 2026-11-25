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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorPageViewEntitySkill
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class BlazorPageViewEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorPageViewEntitySkillTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public BlazorPageViewEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-page-view-entity")
        .FromMarkdown("""
          ---
          name: blazor-page-view-entity
          description: Implements Blazor read-only view entity pages using plain Bootstrap layout, preserving existing .razor.cs data loading, service, and navigation behavior while rendering a structured non-editable detail view. Use when creating or implementing view, detail, inspect, or read-only display entity pages in Blazor, including when an empty or skeleton page already exists and needs its razor markup or code-behind filled in.
          paths:
          - "**/*.razor"
          - "**/*.razor.cs"
          - "**/design.md"
          - "**/ux-tokens.css"
          - "**/ux-base.css"
          - "**/ux-components.css"
          ---

          # View-entity page (Bootstrap)

          For read-only view or detail pages. Not for add or edit forms, search pages, or dialogs.

          ## 1. Read first

          1. `view-entity-sample.razor` and `view-entity-sample.cs`, in this skill's folder. If either can't be read, stop and say which; don't work from memory.
          2. The target `.razor` and `.razor.cs`, and the DTOs and enums they use.
          3. If present: `design.md` (anywhere in the project) and `wwwroot/ux-tokens.css`, `ux-base.css`, `ux-components.css`. Missing ones don't block.

          ## 2. The `.razor.cs` is the source of truth

          It defines loading, service calls, navigation and the DTO. If loading is an empty stub, fill it in through the existing service, following the sample; otherwise leave every existing method, service call (and its parameters) and DTO property as it is. Never invent fields, services, routes or navigation methods.

          ## 3. Layout

          Copy the sample's structure: hero header (`<div class="ux-gradient-primary ux-rounded-lg p-4 mb-4">` with `<h1>` and `<p>`), then a main card (`<div class="card ux-fade-in-up">` with `card-body`), with no extra wrappers. Keep it unless the user asks otherwise; `design.md` informs styling, not layout. Reuse the sample's utility classes when they exist: grep each name as a substring across `wwwroot/**/*.css`, since they are often compound selectors.

          While the DTO is `null`, show only the sample's loading indicator (a `spinner-border` centred in a flex column, with a short message).

          ## 4. Display

          Read-only: no `InputText`, `InputSelect` or other inputs.

          | Property | Display |
          |---|---|
          | string / number | a plain `<div>` under a `<div class="form-label text-uppercase small">` label |
          | bool / status | a badge `<span>` with the sample's classes: `badge-success` for positive, `badge-danger` or `badge-neutral` for negative; never new badge classes |
          | enum | a display label, not the number, from the enum's real members (read its definition), as text or a badge |
          | nullable object | a section inside an `@if` matching the code-behind's null check; none of its fields outside it |
          | collection | `@foreach` (not an indexed `for`) inside an `@if`; each item in a `ux-inner-panel p-3` block with the sample's `row g-3` grid; when empty, a `<span class="text-muted">` message |

          ## 5. Actions

          At the bottom of the card, in a `d-flex justify-content-end gap-2` wrapper: Back (`btn btn-outline`) always, calling the existing cancel or navigation method; Edit (`btn btn-primary`) only when an edit route or method exists.

          ## 6. Never

          - An `@code` block, or a service call, in the `.razor`.
          - Hardcoded colours: inline `Style=` uses `var(--token)`. Existing shared styles, tokens and theme values stay unchanged; keep component CSS minimal.

          ## Definition of Done

          - [ ] Hero header + main card as in the sample, with its utility classes where they exist
          - [ ] Every displayed property exists on the DTO; no inputs; no `@code`
          - [ ] Loading state while the DTO is `null`; data loads through existing methods
          - [ ] Nullable sections guarded; collections use `@foreach` with an empty message
          - [ ] Status uses the sample's badge classes; enum labels from real members
          - [ ] Back and Edit call only existing methods or routes
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
