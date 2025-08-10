using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api.Mappings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Blazor.Api;

public static class RazorFileExtensions
{
    public static IHtmlElement SelectHtmlElement(this IRazorFile razorFile, string selector)
    {
        return razorFile.SelectHtmlElements(selector).SingleOrDefault();
    }

    public static IEnumerable<IHtmlElement> SelectHtmlElements(this IRazorFile razorFile, string selector)
    {
        return razorFile.ChildNodes.OfType<IHtmlElement>().SelectHtmlElements(selector.Split("/", StringSplitOptions.RemoveEmptyEntries));
    }

    public static IEnumerable<IHtmlElement> SelectHtmlElements(this IEnumerable<IHtmlElement> nodes, string[] parts)
    {
        var firstPart = parts.FirstOrDefault();
        var foundNodes = nodes.Where(x => x.Name == firstPart).ToList();
        foreach (var found in foundNodes)
        {
            if (parts.Length == 1)
            {
                yield return found;
            }

            foreach (var foundChildren in found.ChildNodes.OfType<IHtmlElement>().SelectHtmlElements(parts.Skip(1).ToArray()))
            {
                yield return foundChildren;
            }
        }
    }

    public static string InjectServiceProperty(this IBuildsCSharpMembers block, string fullyQualifiedTypeName, string? propertyName = null)
    {
        var type = block.Template.UseType(fullyQualifiedTypeName);
        propertyName ??= type.Length > 2 && type[0] == 'I' && char.IsUpper(type[1]) ? type[1..] : type; // remove 'I' prefix if necessary.

        if (block is IRazorCodeBlock razorCodeBlock)
        {
            razorCodeBlock.RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
        }
        else if (block is ICSharpClass @class)
        {
            if (@class.Properties.Any(x => x.Type == type))
            {
                return propertyName;
            }
            @class.AddProperty(
                type: type,
                name: propertyName ?? type,
                configure: property =>
                {
                    property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
                    property.WithInitialValue("default!");
                });
        }
        return propertyName;
    }

    public static void AddCodeBlockMembers(this IBuildsCSharpMembers block, IRazorComponentTemplate template, IElement componentElement)
    {
        foreach (var child in componentElement.ChildElements)
        {
            if (child.IsPropertyModel())
            {
                block.AddProperty(block.Template.GetTypeName(child.TypeReference), child.Name.ToPropertyName(), property =>
                {
                    property.RepresentsModel(child);
                    if (!string.IsNullOrWhiteSpace(child.Value))
                    {
                        property.WithInitialValue(child.Value);
                    }
                    if (child.AsPropertyModel().HasBindable() || child.AsPropertyModel().HasRouteParameter())
                    {
                        property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Parameter"));
                    }
                    if (child.AsPropertyModel().HasQueryParameter())
                    {
                        property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.SupplyParameterFromQuery"));
                    }
                });
            }

            if (child.IsEventEmitterModel())
            {
                block.AddProperty($"EventCallback{(child.TypeReference.Element != null ? $"<{block.Template.GetTypeName(child.TypeReference)}>" : "")}", child.Name.ToPropertyName(), property =>
                {
                    if (child.AsEventEmitterModel().HasBindable())
                    {
                        property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Parameter"));
                    }
                });
            }

            if (child.IsComponentOperationModel())
            {
                var operation = child.AsComponentOperationModel();
                var methodName = operation.Name.ToPropertyName();
                if (methodName is "OnInitialized" && operation.CallServiceOperationActionTargets().Any())
                {
                    methodName += "Async";
                }
                block.AddMethod(block.Template.GetTypeName(operation.TypeReference), methodName, method =>
                {
                    method.Private();
                    method.RepresentsModel(child);
                    method.AddMetadata("model", operation);
                    if (methodName.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase)/* && operation.CallServiceOperationActionTargets().Any()*/)
                    {
                        method.Async();
                    }

                    if (methodName is "OnInitializedAsync" or "OnInitialized") // TODO: add the others...
                    {
                        method.Protected().Override();
                    }
                    foreach (var parameter in operation.Parameters)
                    {
                        method.AddParameter(block.Template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                        {
                            if (parameter.Value != null)
                            {
                                param.WithDefaultValue(parameter.Value);
                            }
                        });
                    }
                    // NOTE: Needs to be in an OnBuild so that service invocations have been built and can be found through the CSharp reference system:
                    template.RazorFile.OnBuild(file =>
                    {
                        var mappingManager = template.CreateMappingManager();
                        mappingManager.SetFromReplacement(operation, null);

                        // if the name of the opperation parameter is not "ParameterName case" (camel case)
                        // then setup a mapping replacement to make it so. This caters for the cases where the operation parameter in the designer
                        // is not added as camelcase
                        foreach (var parameter in operation.Parameters
                            .Where(p => !p.Name?.Equals(p.Name?.ToParameterName(), StringComparison.InvariantCulture) ?? false))
                        {
                            mappingManager.SetFromReplacement(parameter, parameter.Name.ToParameterName());
                        }

                        var mappings = template.BindingManager.ViewBinding.MappedEnds.Where(x => x.SourceElement?.Id == operation.Id).ToList() ?? new();
                        //var mappedButton = mappings.FirstOrDefault(x => x.TargetPath.Any(p => p.Element.SpecializationType == "Button"));

                        foreach (var action in operation.GetProcessingActions())
                        {
                            if (action.IsInvocationModel() && action.Mappings.Count() == 1)
                            {
                                var operationMapping = action.Mappings.Single();
                                var mappedEnd = operationMapping.MappedEnds.FirstOrDefault(x => x.SourceElement.Id == action.Id);
                                if (mappedEnd == null)
                                {
                                    throw new ElementException(action, "Mapping required for this invocation");
                                }
                                var invocation = mappingManager.GenerateSourceStatementForMapping(operationMapping, mappedEnd);

                                var invStatement = invocation as CSharpInvocationStatement;
                                if (invStatement?.IsAsyncInvocation() == true || mappedEnd?.TargetElement.TypeReference?.Element?.Name == "Task")
                                {
                                    invocation = new CSharpAwaitExpression(invocation);
                                }
                                method.AddStatement(invocation);
                                continue;
                            }

                            if (action.IsCallServiceOperationActionTargetEndModel())
                            {
                                var serviceCall = action.AsCallServiceOperationActionTargetEndModel();
                                var parentElement = ((IElement)serviceCall.Element).ParentElement;
                                var invocationMapping = serviceCall.GetMapInvocationMapping();
                                var targetElement = (IElement)invocationMapping.TargetElement;

                                const string commandSpecializationTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
                                const string querySpecializationTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
                                const string dtoFieldTypeId = "7baed1fd-469b-4980-8fd9-4cefb8331eb2";
                                const string httpSettingsDefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

                                string? serviceName = null;
                                CSharpStatement? invocation = null;

                                if (template.ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() && IsLocalServiceInvocatiion(action, targetElement, parentElement))
                                {
                                    if (method.Name == "LoadCategories" )
                                    {

                                    }
                                    if (targetElement.SpecializationTypeId is commandSpecializationTypeId or querySpecializationTypeId)
                                    {
                                        serviceName = block.InjectServiceProperty(block.Template.GetScopedMediatorInterfaceTemplateName(), "Mediator");
                                        var csharpInvocationStatement = new CSharpInvocationStatement("Send");

                                        block.Template.AddTypeSource(TemplateRoles.Application.Command);
                                        block.Template.AddTypeSource(TemplateRoles.Application.Query);

                                        csharpInvocationStatement.AddArgument(mappingManager.GenerateCreationStatement(invocationMapping));
                                        invocation = csharpInvocationStatement;
                                    }
                                    else //Traditional Service
                                    {
                                        var executorServiceName = block.InjectServiceProperty(block.Template.GetScopedExecutorInterfaceTemplateName(), "ScopedExecutor");

                                        //Service Invocation
                                        if (!block.Template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Interface, targetElement.ParentId, out var serviceInterfaceTemplate))
                                        {
                                            return;
                                        }

                                        // So that the mapping system can resolve the name of the operation from the interface itself:
                                        block.Template.AddTypeSource(serviceInterfaceTemplate.Id);
                                        block.Template.AddTypeSource("Intent.Application.Dtos.DtoModel");
                                        block.Template.AddTypeSource(TemplateRoles.Application.Contracts.Enum);

                                        string tradServiceInterfaceName = block.Template.GetTypeName(serviceInterfaceTemplate);
                                        string tradServiceVariable = tradServiceInterfaceName.Substring(1).ToCamelCase();

                                        serviceName = tradServiceVariable;
                                        var csharpInvocationStatement = mappingManager.GenerateCreationStatement(invocationMapping);

                                        //Scoped Wrapper for Invocation
                                        var csharpInvocationStatementWrapper = new CSharpInvocationStatement($"await {executorServiceName}.ExecuteAsync");
                                        csharpInvocationStatementWrapper.AddArgument(new CSharpLambdaBlock("async sp")
                                            .AddStatement($"var {tradServiceVariable} = sp.GetRequiredService<{tradServiceInterfaceName}>();")
                                            .AddStatements(GetCallServiceOperation(serviceCall, mappingManager, serviceName, csharpInvocationStatement))
                                            );
                                        method.AddStatement(csharpInvocationStatementWrapper);
                                        continue;
                                    }
                                }
                                else
                                {
                                    serviceName = block.InjectServiceProperty(block.Template.GetTypeName(parentElement));

                                    if (targetElement.SpecializationTypeId is commandSpecializationTypeId or querySpecializationTypeId)
                                    {
                                        if (!targetElement.HasStereotype(httpSettingsDefinitionId))
                                        {
                                            throw new ElementException(action, "Target CQRS request is not exposed with HTTP");
                                        }

                                        var nameOfMethodToInvoke = block.Template
                                            .GetAllTypeInfo(parentElement.AsTypeReference())
                                            .Select(x => x.Template)
                                            .OfType<ICSharpTemplate>()
                                            .FirstOrDefault(x => x.RootCodeContext.TryGetReferenceForModel(targetElement.Id, out _))
                                            ?.RootCodeContext.GetReferenceForModel(targetElement.Id).Name;

                                        if (nameOfMethodToInvoke == null)
                                        {
                                            throw new FriendlyException("Unable to resolve the service type for the service call to `" + targetElement.DisplayText + "`. Try installing a module to realize this service (e.g. `Intent.Blazor.HttpClients`)");
                                        }

                                        var csharpInvocationStatement = new CSharpInvocationStatement(nameOfMethodToInvoke);
                                        if (targetElement.ChildElements.Any(x => x.SpecializationTypeId is dtoFieldTypeId))
                                        {
                                            csharpInvocationStatement.AddArgument(mappingManager.GenerateCreationStatement(invocationMapping));
                                        }

                                        invocation = csharpInvocationStatement;
                                    }
                                    else // Proxies
                                    {
                                        invocation = mappingManager.GenerateUpdateStatements(invocationMapping).First();
                                    }
                                }
                                method.AddStatements(GetCallServiceOperation(serviceCall, mappingManager, serviceName, invocation));

                                continue;
                            }

                            if (action.IsNavigationTargetEndModel())
                            {
                                var navigationModel = action.AsNavigationTargetEndModel();
                                if (!navigationModel.Element.AsComponentModel().HasPage())
                                {
                                    throw new ElementException(navigationModel.Element, "Navigation is targeting a Component that isn't a page. Please add the Page stereotype to the targeted Component.");
                                }
                                var route = new RouteManager($"\"{navigationModel.Element.AsComponentModel().GetPage().Route()}\"");

                                var mapping = navigationModel.InternalElement.Mappings.FirstOrDefault();
                                if (mapping != null)
                                {
                                    foreach (var mappedEnd in mapping.MappedEnds)
                                    {
                                        var routeParameter = mappedEnd.TargetElement;

                                        if (route.HasParameterExpression(routeParameter.Name))
                                        {
                                            route.ReplaceParameterExpression(routeParameter.Name, $"{{{mappingManager.GenerateSourceStatementForMapping(mapping, mappedEnd)}}}");
                                        }
                                    }
                                }

                                var serviceName = block.InjectServiceProperty("Microsoft.AspNetCore.Components.NavigationManager");

                                if (route.Route.Contains("{"))
                                {
                                    var adjusted = FixParameterCasing(route.Route, method.Parameters.Select(p => p.Name).ToList());
                                    method.AddStatement($"{serviceName}.NavigateTo(${adjusted});");
                                }
                                else
                                {
                                    method.AddStatement($"{serviceName}.NavigateTo({route.Route});");
                                }

                                continue;
                            }

                            // TODO: This code needs to be in the MudBlazor module. Make this an injectable strategy:
                            if (action.IsShowDialogTargetEndModel())
                            {
                                var navigationModel = action.AsShowDialogTargetEndModel();

                                block.InjectServiceProperty("MudBlazor.IDialogService", "DialogService");

                                var mapping = navigationModel.InternalElement.Mappings.FirstOrDefault();

                                var dialogParameters = new CSharpStatementBlock($"new DialogParameters<{block.Template.GetTypeName(navigationModel)}>");
                                if (mapping != null && mapping.MappedEnds.Count > 0)
                                {
                                    mappingManager.SetToReplacement(navigationModel.Element, null);
                                    foreach (var mappedEnd in mapping.MappedEnds)
                                    {
                                        dialogParameters.AddStatement($"{{ x => x.{mappingManager.GenerateTargetStatementForMapping(mapping, mappedEnd)}, {mappingManager.GenerateSourceStatementForMapping(mapping, mappedEnd)} }},");
                                    }
                                    method.AddStatement(new CSharpAssignmentStatement("var parameters", dialogParameters.WithSemicolon()));
                                }

                                method.AddStatement(new CSharpAssignmentStatement("var dialog", new CSharpAwaitExpression(new CSharpInvocationStatement($"DialogService.ShowAsync<{block.Template.GetTypeName(navigationModel)}>")
                                    .AddArgument($"\"{navigationModel.Name}\"")
                                    .AddArgument("parameters", a =>
                                    {
                                        if (mapping == null || mapping.MappedEnds.Count == 0)
                                            a.Remove();
                                    })
                                    .AddArgument("new DialogOptions() { FullWidth = true }"))));
                                method.AddStatement(new CSharpAssignmentStatement("var result", new CSharpAwaitExpression(new CSharpStatement($"dialog.Result;"))));
                                method.AddStatement(new CSharpIfStatement("result.Canceled").AddStatement("return;"));

                                const string StaticMappableResponseId = "f815a2ce-59e4-4e53-bea1-dd4ba46a8b7a";

                                mappingManager.SetFromReplacement(navigationModel, null);
                                mappingManager.SetFromReplacement(new StaticMetadata(StaticMappableResponseId), "result.Data");

                                continue;
                            }
                        }

                        if (method.Statements.Any(x => x.ToString().Contains("await ")))
                        {
                            method.Async();
                        }
                    });
                });
            }

            if (child.IsModelDefinitionModel())
            {
                var modelDefinition = child.AsModelDefinitionModel();
                block.AddClass(modelDefinition.Name, @class =>
                {
                    foreach (var genericType in modelDefinition.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }
                    foreach (var propertyModel in modelDefinition.Properties)
                    {
                        @class.AddProperty(propertyModel);
                    }
                });
            }
        }

        //foreach (var associationEnd in componentElement.AssociatedElements)
        //{
        //    if (associationEnd.IsNavigationTargetEndModel())
        //    {
        //        var navigationModel = associationEnd.AsNavigationTargetEndModel();
        //        block.AddMethod("void", associationEnd.Name.ToPropertyName(), method =>
        //        {
        //            method.Private();
        //            var routeManager = new RouteManager($"\"{navigationModel.Element.AsComponentModel().GetPage().Route()}\"");
        //            foreach (var parameter in navigationModel.Parameters)
        //            {
        //                method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
        //                {
        //                    if (parameter.Value != null)
        //                    {
        //                        param.WithDefaultValue(parameter.Value);
        //                    }
        //                });
        //                if (routeManager.HasParameterExpression(parameter.Name))
        //                {
        //                    routeManager.ReplaceParameterExpression(parameter.Name, $"{{{parameter.Name.ToParameterName()}}}");
        //                }
        //            }

        //            var route = routeManager.Route;
        //            if (route.Contains("{"))
        //            {
        //                route = $"${route}";
        //            }

        //            template.RazorFile.AddInjectDirective("NavigationManager");
        //            method.AddStatement($"NavigationManager.NavigateTo({route});");
        //        });
        //    }
        //}
    }

    static string FixParameterCasing(string route, List<string> localVariableNames)
    {
        // Matches {Name} or {Name:type}
        var regex = new Regex(@"\{(?<name>[^}:]+)(:[^}]*)?\}", RegexOptions.Compiled);

        return regex.Replace(route, match =>
        {
            string name = match.Groups["name"].Value;

            // Find matching correct name (case-insensitive)
            string correctName = localVariableNames.Find(n =>
                string.Equals(n, name, StringComparison.OrdinalIgnoreCase));

            // Always output without :type
            if (!string.IsNullOrEmpty(correctName))
                return $"{{{correctName}}}";

            return $"{{{name}}}";
        });
    }

    private static IEnumerable<CSharpStatement> GetCallServiceOperation(CallServiceOperationActionTargetEndModel serviceCall
        , CSharpClassMappingManager mappingManager
        , string serviceName
        , CSharpStatement invocation)
    {
        var result = new List<CSharpStatement>();
        if (serviceCall.GetMapResponseMapping() != null)
        {
            var responseStaticElementId = "2f68b1a4-a523-4987-b3da-f35e6e8e146b";
            if (serviceCall.GetMapResponseMapping().MappedEnds.Count == 1 && serviceCall.GetMapResponseMapping().MappedEnds.Single().SourceElement.Id == responseStaticElementId)
            {
                result.Add(new CSharpAssignmentStatement(
                    lhs: mappingManager.GenerateTargetStatementForMapping(serviceCall.GetMapResponseMapping(), serviceCall.GetMapResponseMapping().MappedEnds.Single()),
                    rhs: new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation))));
            }
            else
            {
                var variableName = serviceCall.TypeReference.Element.TypeReference.IsCollection ? serviceCall.TypeReference.Element.TypeReference.Element.Name.Pluralize().ToLocalVariableName() : serviceCall.TypeReference.Element.TypeReference.Element.Name.ToLocalVariableName();
                result.Add(new CSharpAssignmentStatement($"var {variableName}", new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation))));
                mappingManager.SetFromReplacement(new StaticMetadata(responseStaticElementId), variableName);
                var response = mappingManager.GenerateUpdateStatements(serviceCall.GetMapResponseMapping());
                foreach (var statement in response)
                {
                    statement.WithSemicolon();
                }

                result.AddRange(response);
            }
        }
        else
        {
            result.Add(new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation)));
        }
        return result;
    }

    private static bool IsLocalServiceInvocatiion(IElement? action, IElement targetElement, IElement parentElement)
    {
        const string commandSpecializationTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        const string querySpecializationTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
        const string serviceOperationSpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb";

        if (targetElement.SpecializationTypeId is not (commandSpecializationTypeId or querySpecializationTypeId or serviceOperationSpecializationTypeId))
        {
            return false;
        }
        
        if (parentElement.Application.Id == action?.Application?.Id)
        {
            return true;
        }
        return false;
    }

    private static List<IElement> GetProcessingActions(this ComponentOperationModel operation)
    {
        var processingActions = operation.InternalElement.ChildElements.ToList();
        foreach (var associationEnd in operation.InternalElement.AssociatedElements.OrderBy(x => x.Order))
        {
            processingActions.Insert(associationEnd.Order, associationEnd);
        }

        return processingActions;
    }

    public static void AddMappingReplacement(this IRazorFileNode node, IMetadataModel model, string replacementString)
    {
        var registry = node.HasMetadata("mapping-replacements")
            ? node.GetMetadata<IDictionary<IMetadataModel, string>>("mapping-replacements")
            : new Dictionary<IMetadataModel, string>();

        registry.TryAdd(model, replacementString);

        node.AddMetadata("mapping-replacements", registry);
    }

    public static string? GetHrefRoute(this BindingManager bindingManager, IList<IElementToElementMappedEnd> mappingEnds)
    {
        if (mappingEnds.Any())
        {
            var route = new RouteManager(mappingEnds[0].SourcePath != null
                ? $"{mappingEnds[0].SourcePath.Last().Element.AsNavigationTargetEndModel().TypeReference.Element.AsComponentModel().GetPage().Route()}"
                : mappingEnds[0].MappingExpression);
            var complexRoute = false;
            foreach (var mappedEnd in mappingEnds)
            {
                var routeParameter = ((IElement)mappedEnd.TargetElement).MappedToElements.FirstOrDefault()?.TargetElement;

                if (routeParameter != null && route.HasParameterExpression(routeParameter.Name))
                {
                    var binding = bindingManager.GetBinding(mappedEnd);
                    route.ReplaceParameterExpression(routeParameter.Name, $"{{{binding}}}");
                    complexRoute = true;
                }
            }

            return complexRoute ? $"@($\"{route.Route}\")" : route.Route;
        }

        return null;
    }

    public static IDictionary<IMetadataModel, string> GetMappingReplacements(this IRazorFileNode node)
    {
        var result = new Dictionary<IMetadataModel, string>();
        foreach (var n in GetAllNodesInHierarchy(node))
        {
            if (n.HasMetadata("mapping-replacements"))
            {
                foreach (var mappingReplacement in n.GetMetadata<IDictionary<IMetadataModel, string>>("mapping-replacements"))
                {
                    result.TryAdd(mappingReplacement.Key, mappingReplacement.Value);
                }
            }
        }

        return result;
    }

    public static IEnumerable<IRazorFileNode> GetAllNodesInHierarchy(this IRazorFileNode node)
    {
        yield return node;
        if (node.Parent != null)
        {
            foreach (var parentNode in GetAllNodesInHierarchy(node.Parent))
            {
                yield return parentNode;
            }
        }
    }

    public static string AddProcessingFieldName(this ICSharpClassMethodDeclaration method)
    {
        var parent = ((IBuildsCSharpMembers)method.Parent);
        var processingFieldName = $"{method.Name}Processing".ToPrivateMemberName();
        parent.AddField("bool", processingFieldName, f => f.WithAssignment("false"));
        //parent.InsertField(parent.IndexOf(method), "bool", processingFieldName, f => f.WithAssignment("false"));
        ((CSharpTryBlock)method.FindStatement(x => x is CSharpTryBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "true").WithSemicolon());
        ((CSharpFinallyBlock)method.FindStatement(x => x is CSharpFinallyBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "false").WithSemicolon());
        return processingFieldName;
    }
}

public record StaticMetadata(string id) : IMetadataModel
{
    public string Id { get; } = id;
}