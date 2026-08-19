using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentStyle
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class RazorComponentStyleTemplate : IntentTemplateBase<ComponentModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorComponentStyleTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public RazorComponentStyleTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: RazorComponentTemplate.GetOutputFileName(Model),
                fileExtension: "razor.css",
                relativeLocation: this.GetFolderPath()
            );
        }

        /// <summary>
        /// When set, seeds this component's companion CSS-isolation file with this literal string.
        /// </summary>
        public string? StyleContentOverride { get; set; }

    }
}
