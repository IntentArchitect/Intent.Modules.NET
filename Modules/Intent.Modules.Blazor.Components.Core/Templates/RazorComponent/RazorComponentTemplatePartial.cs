using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using ComponentModel = Intent.Modelers.UI.Api.ComponentModel;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RazorComponentTemplate : CSharpTemplateBase<ComponentModel>, IDeclareUsings, IRazorComponentTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource("Intent.Blazor.HttpClients.DtoContract");
            AddTypeSource("Intent.Blazor.HttpClients.ServiceContract");
            AddTypeSource(TemplateId);
            RazorFile = new RazorFile(this);
            BindingManager = new BindingManager(this, Model.View.InternalElement.Mappings.FirstOrDefault());
            ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);// new RazorComponentBuilderProvider(this);

            RazorFile.Configure(file =>
            {
                if (Model.HasPage())
                {
                    RazorFile.AddPageDirective(Model.GetPage().Route());
                    if (!string.IsNullOrWhiteSpace(Model.GetPage().Title()))
                    {
                        RazorFile.AddHtmlElement("PageTitle", x => x.WithText(Model.GetPage().Title()));
                    }
                }

                foreach (var component in Model.View.InternalElement.ChildElements)
                {
                    ComponentBuilderProvider.ResolveFor(component).BuildComponent(component, RazorFile);
                }

                file.AddCodeBlock(block =>
                {
                    block.AddCodeBlockMembers(this, Model.InternalElement);
                    if (Model.HasPage())
                    {
                        foreach (var declaration in block.Declarations)
                        {
                            if (declaration is CSharpProperty property && new RouteManager(Model.GetPage().Route()).HasParameterExpression(property.Name))
                            {
                                property.AddAttribute("Parameter");
                            }
                        }
                    }
                });
            });
        }

        public BindingManager BindingManager { get; }
        public RazorFile RazorFile { get; set; }
        public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }

        public void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null)
        {
            RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
        }

        public CSharpClassMappingManager CreateMappingManager()
        {
            var mappingManager = new CSharpClassMappingManager(this);
            mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(this));
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

        public override string TransformText()
        {
            var razorFile = RazorFile.Build();
            foreach (var @using in this.ResolveAllUsings(
                             "System",
                             "System.Collections.Generic"
                             ).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                razorFile.AddUsing(@using);
            }

            return razorFile.ToString();
        }

        public override string RunTemplate()
        {
            return TransformText();
        }
    }

    public class CallServiceOperationMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpTemplate _template;

        public CallServiceOperationMappingResolver(ICSharpTemplate template)
        {
            _template = template;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Model.SpecializationType == "Operation")
            {
                return new MethodInvocationMapping(mappingModel, _template);
            }

            if (mappingModel.Model.TypeReference?.Element?.SpecializationType == "Command")
            {
                return new ObjectInitializationMapping(mappingModel, _template);
            }
            return null;
        }
    }
}