using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class RazorComponentTemplate : IntentTemplateBase<ComponentModel>
    {
        private readonly IComponentRenderer _componentRenderer;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model, IComponentRenderer componentRenderer) : base(TemplateId, outputTarget, model)
        {
            _componentRenderer = componentRenderer;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "razor"
            );
        }

    }

    [IntentIgnore]
    public interface IComponentRenderer
    {
        string Render(IElement component);
    }
}