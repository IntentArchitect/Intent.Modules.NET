using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.AI.UIModellingInstructions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UIModellingInstructionsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.UIModellingInstructionsTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UIModellingInstructionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"ui-modeling-blazor.instructions")
                .FromMarkdown("""
---
name: ui-modeling-blazor.instructions
description: General guidance for AI on how to model UIs for Blazor.
---

## General Rules

- UI Pages / Components should be modeled in the UI Designer.
- Any navigation between Pages should be modeled.
- Pages / Components which are designer to talk to services should model those service interactions.
- If Pages / Components talk to Services the Service Package (where the services are modeled) should be added as a reference in the UI Designer (if its not already there).

""");

            if (ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() || ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
            {
            }
            if (ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly() || ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveAuto())
            {   
                MarkdownFile.WithSection("WASM Specific Guidance", section => {
                    section.WithListItem("In the service design Do not model services using explicit proxies, rather model against the actual commands and queries.");
                    section.WithListItem("Don't look or expect to find anything in the WASM Projects services designer.");
                    section.WithListItem("If you want to connect to services in other applications you must add a Service Reference in the WASM Applications UI Designer to the Service Application's Service package.");
                });
            }

        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}