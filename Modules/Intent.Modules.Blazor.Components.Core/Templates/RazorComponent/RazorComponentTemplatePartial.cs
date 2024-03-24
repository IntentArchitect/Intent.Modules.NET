using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
using Intent.Modules.Common;
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
    public class RazorComponentTemplate : CSharpTemplateBase<ComponentModel>, IDeclareUsings
    {
        private readonly IComponentRendererResolver _componentResolver;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            //Types = new CSharpTypeResolver(
            //    defaultCollectionFormatter: CSharpCollectionFormatter.Create("System.Collections.Generic.IEnumerable<{0}>"),
            //    defaultNullableFormatter: CSharpNullableFormatter.Create(OutputTarget.GetProject()));
            AddTypeSource("Intent.Blazor.HttpClients.DtoContract");
            AddTypeSource(TemplateId);
            RazorFile = new RazorFile();
            _componentResolver = new ComponentRendererResolver(this);
            ViewBinding = Model.View.InternalElement.Mappings.FirstOrDefault();
            RazorFile.Configure(file =>
            {
                if (Model.HasPage())
                {
                    RazorFile.AddPageDirective(Model.GetPage().Route());
                }

                foreach (var component in Model.View.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(component).Render(component, RazorFile);
                }

                file.AddCodeBlock("code", block =>
                {
                    foreach (var child in Model.InternalElement.ChildElements)
                    {
                        if (child.IsPropertyModel())
                        {
                            block.AddProperty(GetTypeName(child.TypeReference), child.Name.ToPropertyName(), property =>
                            {
                                if (!string.IsNullOrWhiteSpace(child.Value))
                                {
                                    property.WithInitialValue(child.Value);
                                }
                                if (child.AsPropertyModel().HasBindable())
                                {
                                    property.AddAttribute("Parameter");
                                }
                            });
                        }

                        if (child.IsEventEmitterModel())
                        {
                            block.AddProperty($"EventCallback{(child.TypeReference.Element != null ? $"<{GetTypeName(child.TypeReference)}>" : "")}", child.Name.ToPropertyName(), property =>
                            {
                                if (child.AsEventEmitterModel().HasBindable())
                                {
                                    property.AddAttribute("Parameter");
                                }
                            });
                        }

                        if (child.IsOperationModel())
                        {
                            block.AddMethod(GetTypeName(child.TypeReference), child.Name.ToPropertyName(), method =>
                            {
                                if (child.Name.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    //method.Async(); // doesn't work unless we set class parent
                                    method.WithReturnType(method.ReturnType == "void" ? "Task" : $"Task<{method.ReturnType}>");
                                }
                                foreach (var parameter in child.AsOperationModel().Parameters)
                                {
                                    method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                                    {
                                        if (parameter.Value != null)
                                        {
                                            param.WithDefaultValue(parameter.Value);
                                        }
                                    });
                                }
                            });
                        }
                    }
                });
            });
        }

        public IElementToElementMapping ViewBinding { get; }

        public CSharpClassMappingManager CreateMappingManager()
        {
            var mappingManager = new CSharpClassMappingManager(this);
            mappingManager.SetFromReplacement(Model, null);
            return mappingManager;
        }

        public string GetBinding(IElementToElementMappedEnd mappedEnd, CSharpClassMappingManager mappingManager = null)
        {
            if (mappedEnd == null)
            {
                return null;
            }

            return (mappingManager ?? CreateMappingManager()).GenerateSourceStatementForMapping(ViewBinding, mappedEnd)?.ToString();
        }

        public string GetElementBinding(IMetadataModel model, out IElementToElementMappedEnd mappedEnd, CSharpClassMappingManager mappingManager = null)
        {
            mappedEnd = ViewBinding.MappedEnds.SingleOrDefault(x => x.TargetElement.Id == model.Id);
            return GetBinding(mappedEnd, mappingManager);
        }

        public string GetElementBinding(IMetadataModel model, CSharpClassMappingManager mappingManager = null)
        {
            return GetElementBinding(model, out _, mappingManager);
        }

        public string GetStereotypePropertyBinding(IMetadataModel model, string propertyName, CSharpClassMappingManager mappingManager = null)
        {
            var mappedEnd = ViewBinding.MappedEnds.FirstOrDefault(x => x.TargetPath.Any(x => x.Id == model.Id) && x.TargetPath.Last().Name == propertyName);
            return GetBinding(mappedEnd, mappingManager);
        }

        public RazorFile RazorFile { get; set; }

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
            foreach (var @using in DependencyUsings.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
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
}