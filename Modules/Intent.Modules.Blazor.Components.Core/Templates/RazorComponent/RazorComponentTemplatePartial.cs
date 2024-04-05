using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Components.Core.Templates.ComponentRenderer;
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
        private readonly IRazorComponentBuilderResolver _componentResolver;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource("Intent.Blazor.HttpClients.DtoContract");
            AddTypeSource("Intent.Blazor.HttpClients.ServiceContract");
            AddTypeSource(TemplateId);
            BlazorFile = new BlazorFile(this);
            _componentResolver = new ComponentRendererResolver(this);

            _componentResolver.Register(FormModel.SpecializationTypeId, new FormComponentRenderer(_componentResolver, this));
            _componentResolver.Register(TextInputModel.SpecializationTypeId, new TextInputComponentRenderer(_componentResolver, this));
            _componentResolver.Register(ButtonModel.SpecializationTypeId, new ButtonRenderer(_componentResolver, this));
            _componentResolver.Register(ContainerModel.SpecializationTypeId, new ContainerRenderer(_componentResolver, this));
            _componentResolver.Register(TableModel.SpecializationTypeId, new TableRenderer(_componentResolver, this));
            _componentResolver.Register(TextModel.SpecializationTypeId, new TextRenderer(_componentResolver, this));
            _componentResolver.Register(DisplayComponentModel.SpecializationTypeId, new CustomComponentRenderer(_componentResolver, this));

            ViewBinding = Model.View.InternalElement.Mappings.FirstOrDefault();
            BlazorFile.Configure(file =>
            {
                if (Model.HasPage())
                {
                    BlazorFile.AddPageDirective(Model.GetPage().Route());
                }

                foreach (var component in Model.View.InternalElement.ChildElements)
                {
                    _componentResolver.ResolveFor(component).BuildComponent(component, BlazorFile);
                }

                file.AddCodeBlock(block =>
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
                                if (child.AsPropertyModel().HasBindable() || (Model.TryGetPage(out var pageSettings) && new RouteManager(pageSettings.Route()).HasParameterExpression(property.Name)))
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

                        if (child.IsComponentOperationModel())
                        {
                            var operation = child.AsComponentOperationModel();
                            block.AddMethod(GetTypeName(operation.TypeReference), operation.Name.ToPropertyName(), method =>
                            {
                                method.RepresentsModel(child); // throws exception because parent Class not set. Refactor CSharp builder to accomodate
                                if (operation.Name.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    method.Async();
                                }

                                if (operation.Name is "OnInitializedAsync" or "OnInitialized")
                                {
                                    method.Protected().Override();
                                }
                                foreach (var parameter in operation.Parameters)
                                {
                                    method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                                    {
                                        if (parameter.Value != null)
                                        {
                                            param.WithDefaultValue(parameter.Value);
                                        }
                                    });
                                }

                                var mappingManager = CreateMappingManager();

                                foreach (var serviceCall in operation.CallServiceOperationActionTargets())
                                {
                                    method.Async();
                                    var serviceName = ((IElement)serviceCall.Element).ParentElement.Name.ToPropertyName();
                                    file.AddInjectDirective(GetTypeName(((IElement)serviceCall.Element).ParentElement), serviceName);
                                    var invocation = mappingManager.GenerateUpdateStatements(serviceCall.GetMapInvocationMapping()).First();
                                    if (serviceCall.GetMapResponseMapping() != null)
                                    {
                                        if (serviceCall.GetMapResponseMapping().MappedEnds.Count == 1 && serviceCall.GetMapResponseMapping().MappedEnds.Single().SourceElement.Id == serviceCall.Id)
                                        {
                                            method.AddStatement(new CSharpAssignmentStatement(mappingManager.GenerateTargetStatementForMapping(serviceCall.GetMapResponseMapping(), serviceCall.GetMapResponseMapping().MappedEnds.Single()), new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                        }
                                        else
                                        {
                                            method.AddStatement(new CSharpAssignmentStatement($"var {serviceCall.Name.ToLocalVariableName()}", new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                            var response = mappingManager.GenerateUpdateStatements(serviceCall.GetMapResponseMapping());
                                            method.AddStatements(response);
                                        }
                                    }
                                    else
                                    {
                                        method.AddStatement(new CSharpAccessMemberStatement($"await {serviceName}", invocation));
                                    }
                                }

                                foreach (var navigationModel in operation.NavigateToComponents())
                                {
                                    method.Private();
                                    var route = $"\"{navigationModel.Element.AsComponentModel().GetPage().Route()}\"";
                                    if (route.Contains("{"))
                                    {
                                        route = $"${route}";
                                    }
                                    foreach (var parameter in navigationModel.Parameters)
                                    {
                                        method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                                        {
                                            if (parameter.Value != null)
                                            {
                                                param.WithDefaultValue(parameter.Value);
                                            }
                                        });
                                        var replaceIndex = route.ToLower().Contains($"{{{parameter.Name.ToLower()}}}")
                                            ? route.ToLower().IndexOf($"{{{parameter.Name.ToLower()}}}", StringComparison.Ordinal)
                                            : route.ToLower().IndexOf($"{{{parameter.Name.ToLower()}:", StringComparison.Ordinal);
                                        if (replaceIndex != -1)
                                        {
                                            route = route.Remove(replaceIndex, route.IndexOf('}', replaceIndex) + 1 - replaceIndex)
                                                .Insert(replaceIndex, $"{{{parameter.Name.ToParameterName()}}}");
                                        }
                                    }

                                    file.AddInjectDirective("NavigationManager");
                                    method.AddStatement($"NavigationManager.NavigateTo({route});");
                                }
                            });
                        }
                    }

                    foreach (var associationEnd in Model.InternalElement.AssociatedElements)
                    {
                        if (associationEnd.IsNavigationTargetEndModel())
                        {
                            var navigationModel = associationEnd.AsNavigationTargetEndModel();
                            block.AddMethod("void", associationEnd.Name.ToPropertyName(), method =>
                            { 
                                method.Private();
                                var routeManager = new RouteManager($"\"{navigationModel.Element.AsComponentModel().GetPage().Route()}\"");
                                foreach (var parameter in navigationModel.Parameters)
                                {
                                    method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                                    {
                                        if (parameter.Value != null)
                                        {
                                            param.WithDefaultValue(parameter.Value);
                                        }
                                    });
                                    if (routeManager.HasParameterExpression(parameter.Name))
                                    {
                                        routeManager.ReplaceParameterExpression(parameter.Name, $"{{{parameter.Name.ToParameterName()}}}");
                                    }
                                }

                                var route = routeManager.Route;
                                if (route.Contains("{"))
                                {
                                    route = $"${route}";
                                }

                                file.AddInjectDirective("NavigationManager");
                                method.AddStatement($"NavigationManager.NavigateTo({route});");
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
            mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(this));
            mappingManager.SetFromReplacement(Model, null);
            mappingManager.SetToReplacement(Model, null);
            return mappingManager;
        }

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

            return (mappingManager ?? CreateMappingManager()).GenerateSourceStatementForMapping(ViewBinding, mappedEnd)?.ToString();
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

        public BlazorFile BlazorFile { get; set; }

        public IRazorComponentBuilderResolver ComponentBuilderResolver => _componentResolver;

        RazorFile IRazorComponentTemplate.BlazorFile => BlazorFile;

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