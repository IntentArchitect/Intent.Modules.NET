using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Api.Mappings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class RazorLayoutTemplate : CSharpTemplateBase<LayoutModel>, IRazorComponentTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutTemplate";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public RazorLayoutTemplate(IOutputTarget outputTarget, LayoutModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = new RazorFile(this);
            BindingManager = new BindingManager(this, Model.InternalElement.Mappings.FirstOrDefault());
            ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);

            RazorFile.Configure(file =>
            {
                file.AddInheritsDirective("LayoutComponentBase");

                ComponentBuilderProvider.ResolveFor(Model.InternalElement)
                    .BuildComponent(Model.InternalElement, RazorFile);

                if (file.ChildNodes.All(x => x is not HtmlElement))
                {
                    file.AddHtmlElement("div", div => div.WithText("@Body"));
                }

                file.AddCodeBlock(block =>
                {
                    block.AddCodeBlockMembers(this, Model.InternalElement);
                });
            });
        }

        public RazorFile RazorFile { get; set; }
        public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }

        public BindingManager BindingManager { get; }

        public void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null)
        {
            RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
        }

        public CSharpClassMappingManager CreateMappingManager()
        {
            var mappingManager = new CSharpClassMappingManager(this);
            mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(this));
            mappingManager.AddMappingResolver(new RazorBindingMappingResolver(this));
            mappingManager.SetFromReplacement(Model, null);
            mappingManager.SetToReplacement(Model, null);
            return mappingManager;
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath(),
                fileExtension: "razor"
            );
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return RazorFile.Build().ToString();
        }

        public override string RunTemplate()
        {
            return TransformText();
        }
    }
}