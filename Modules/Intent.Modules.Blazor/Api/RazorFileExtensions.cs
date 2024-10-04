using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
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

    public static void InjectServiceProperty(this IBuildsCSharpMembers block, string fullyQualifiedTypeName, string? propertyName = null)
    {
        if (block is IRazorCodeBlock razorCodeBlock)
        {
            razorCodeBlock.RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
        }
        else if (block is ICSharpClass @class)
        {
            var type = block.Template.UseType(fullyQualifiedTypeName);
            if (@class.Properties.Any(x => x.Type == type))
            {
                return;
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
    }

    public static void AddCodeBlockMembers(this IBuildsCSharpMembers block, IRazorComponentTemplate template, IElement componentElement)
    {
        foreach (var child in componentElement.ChildElements)
        {
            if (child.IsPropertyModel())
            {
                block.AddProperty(block.Template.GetTypeName(child.TypeReference), child.Name.ToPropertyName(), property =>
                {
                    if (!string.IsNullOrWhiteSpace(child.Value))
                    {
                        property.WithInitialValue(child.Value);
                    }
                    if (child.AsPropertyModel().HasBindable() || child.AsPropertyModel().HasRouteParameter())
                    {
                        property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Parameter"));
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
                    if (methodName.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase)/* && operation.CallServiceOperationActionTargets().Any()*/)
                    {
                        method.Async();
                    }

                    if (methodName is "OnInitializedAsync" or "OnInitialized")
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
                        var mappings = template.BindingManager.ViewBinding.MappedEnds.Where(x => x.SourceElement?.Id == operation.Id).ToList() ?? new();
                        var mappedButton = mappings.FirstOrDefault(x => x.TargetPath.Any(p => p.Element.SpecializationType == "Button"));

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
                                var serviceName = ((IElement)serviceCall.Element).ParentElement.Name.ToPropertyName();
                                block.InjectServiceProperty(block.Template.GetTypeName(((IElement)serviceCall.Element).ParentElement), serviceName);
                                var invocation = mappingManager.GenerateUpdateStatements(serviceCall.GetMapInvocationMapping()).First();
                                if (serviceCall.GetMapResponseMapping() != null)
                                {
                                    var responseStaticElementId = "2f68b1a4-a523-4987-b3da-f35e6e8e146b";
                                    if (serviceCall.GetMapResponseMapping().MappedEnds.Count == 1 && serviceCall.GetMapResponseMapping().MappedEnds.Single().SourceElement.Id == responseStaticElementId)
                                    {
                                        method.AddStatement(new CSharpAssignmentStatement(
                                            lhs: mappingManager.GenerateTargetStatementForMapping(serviceCall.GetMapResponseMapping(), serviceCall.GetMapResponseMapping().MappedEnds.Single()),
                                            rhs: new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation))));
                                    }
                                    else
                                    {
                                        var variableName = serviceCall.TypeReference.Element.TypeReference.Element.Name.ToLocalVariableName();
                                        method.AddStatement(new CSharpAssignmentStatement($"var {variableName}", new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation))));
                                        mappingManager.SetFromReplacement(new StaticMetadata(responseStaticElementId), variableName);
                                        var response = mappingManager.GenerateUpdateStatements(serviceCall.GetMapResponseMapping());
                                        foreach (var statement in response)
                                        {
                                            statement.WithSemicolon();
                                        }

                                        method.AddStatements(response);
                                    }
                                }
                                else
                                {
                                    method.AddStatement(new CSharpAwaitExpression(new CSharpAccessMemberStatement($"{serviceName}", invocation)));
                                }
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

                                block.InjectServiceProperty("Microsoft.AspNetCore.Components.NavigationManager");
                                method.AddStatement($"NavigationManager.NavigateTo({(route.Route.Contains("{") ? $"${route.Route}" : route.Route)});");
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
                                continue;
                            }
                        }

                        var hasImplicitEventEmitter = componentElement.ChildElements.SingleOrDefault(x => method.Name == $"On{x.Name.ToPropertyName()}");
                        if (hasImplicitEventEmitter != null)
                        {
                            method.Async();
                            method.AddStatement(new CSharpAwaitExpression(new CSharpStatement($"{hasImplicitEventEmitter.Name.ToPropertyName()}.InvokeAsync();")));
                        }

                        if (method.Statements.Any(x => x.ToString().Contains("await ")))
                        {
                            method.Async();
                            method.AddTryBlock(tryBlock =>
                            {
                                foreach (var statement in method.Statements.Where(x => x != tryBlock).ToList())
                                {
                                    statement.Remove();
                                    tryBlock.AddStatement(statement);
                                }
                            });


                            method.AddCatchBlock(catchBlock =>
                            {
                                catchBlock.WithExceptionType(block.Template.UseType("System.Exception")).WithParameterName("e");
                                //if (errorMessageProperty != null)
                                //{
                                //    catchBlock.AddStatement(new CSharpAssignmentStatement(errorMessageProperty, "e.Message"), s => s.WithSemicolon());
                                //}

                                block.InjectServiceProperty("MudBlazor.ISnackbar", "Snackbar");
                                catchBlock.AddStatement($"Snackbar.Add(e.Message, {block.Template.UseType("MudBlazor.Severity")}.Error);");
                            });

                            if (mappings.Count == 0)
                            {
                                method.AddStatement("StateHasChanged();");
                            }
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