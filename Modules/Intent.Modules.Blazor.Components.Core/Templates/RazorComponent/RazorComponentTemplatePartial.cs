using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
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
    public class BindingManager
    {
        private readonly IRazorComponentTemplate _componentTemplate;

        public BindingManager(IRazorComponentTemplate template, IElementToElementMapping viewBinding)
        {
            _componentTemplate = template;
            ViewBinding = viewBinding;
        }

        public IElementToElementMapping ViewBinding { get; }

        
        public string GetCodeDirective(IElementToElementMappedEnd mappedEnd, CSharpClassMappingManager mappingManager = null)
        {
            if (mappedEnd == null)
            {
                return null;
            }

            var binding = GetBinding(mappedEnd, mappingManager);
            return binding.Contains(' ') ? $"@({binding})" : $"@{binding}";
        }

        public string GetBinding(IElementToElementMappedEnd mappedEnd, CSharpClassMappingManager mappingManager = null)
        {
            if (mappedEnd == null)
            {
                return null;
            }

            return (mappingManager ?? _componentTemplate.CreateMappingManager()).GenerateSourceStatementForMapping(ViewBinding, mappedEnd)?.ToString();
        }

        public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model)
        {
            return ViewBinding.MappedEnds.SingleOrDefault(x => x.TargetElement?.Id == model.Id);
        }

        public IElementToElementMappedEnd GetMappedEndFor(IMetadataModel model, string stereotypePropertyName)
        {
            return ViewBinding.MappedEnds.SingleOrDefault(x => x.TargetPath.Any(x => x.Id == model.Id) && x.TargetPath.Last().Name == stereotypePropertyName);
        }

        public string GetElementBinding(IMetadataModel model, CSharpClassMappingManager mappingManager = null)
        {
            var mappedEnd = GetMappedEndFor(model);
            return GetBinding(mappedEnd, mappingManager);
        }

        public string GetStereotypePropertyBinding(IMetadataModel model, string propertyName, CSharpClassMappingManager mappingManager = null)
        {
            var mappedEnd = GetMappedEndFor(model, propertyName);
            return GetBinding(mappedEnd, mappingManager);
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RazorComponentComponentTemplate : CSharpTemplateBase<ComponentModel>, IDeclareUsings, IRazorComponentTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource("Intent.Blazor.HttpClients.DtoContract");
            AddTypeSource("Intent.Blazor.HttpClients.ServiceContract");
            AddTypeSource(TemplateId);
            BlazorFile = new BlazorFile(this);
            BindingManager = new BindingManager(this, Model.View.InternalElement.Mappings.FirstOrDefault());
            ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);// new RazorComponentBuilderProvider(this);

            BlazorFile.Configure(file =>
            {
                if (Model.HasPage())
                {
                    BlazorFile.AddPageDirective(Model.GetPage().Route());
                }

                foreach (var component in Model.View.InternalElement.ChildElements)
                {
                    ComponentBuilderProvider.ResolveFor(component).BuildComponent(component, BlazorFile);
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
        public BlazorFile BlazorFile { get; set; }
        public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }
        RazorFile IRazorComponentTemplate.BlazorFile => BlazorFile;

        public void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null)
        {
            BlazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
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
            var razorFile = BlazorFile.Build();
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