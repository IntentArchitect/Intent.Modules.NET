using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Blazor.Api;

public static class RazorFileExtensions
{
    public static HtmlElement SelectHtmlElement(this RazorFile razorFile, string selector)
    {
        return razorFile.SelectHtmlElements(selector).SingleOrDefault();
    }

    public static IEnumerable<HtmlElement> SelectHtmlElements(this RazorFile razorFile, string selector)
    {
        return razorFile.ChildNodes.OfType<HtmlElement>().SelectHtmlElements(selector.Split("/", StringSplitOptions.RemoveEmptyEntries));
    }

    public static IEnumerable<HtmlElement> SelectHtmlElements(this IEnumerable<HtmlElement> nodes, string[] parts)
    {
        var firstPart = parts.FirstOrDefault();
        var foundNodes = nodes.Where(x => x.Name == firstPart).ToList();
        foreach (var found in foundNodes)
        {
            if (parts.Length == 1)
            {
                yield return found;
            }

            foreach (var foundChildren in found.ChildNodes.OfType<HtmlElement>().SelectHtmlElements(parts.Skip(1).ToArray()))
            {
                yield return foundChildren;
            }
        }
    }

    public static void AddCodeBlockMembers(this IBuildsCSharpMembers block, IRazorComponentTemplate template, IElement componentElement)
    {
        foreach (var child in componentElement.ChildElements)
        {
            if (child.IsPropertyModel())
            {
                block.AddProperty(template.GetTypeName(child.TypeReference), child.Name.ToPropertyName(), property =>
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
                block.AddProperty($"EventCallback{(child.TypeReference.Element != null ? $"<{template.GetTypeName(child.TypeReference)}>" : "")}", child.Name.ToPropertyName(), property =>
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
                var methodName = operation.Name.ToPropertyName();
                if (methodName is "OnInitialized" && operation.CallServiceOperationActionTargets().Any())
                {
                    methodName += "Async";
                }
                block.AddMethod(template.GetTypeName(operation.TypeReference), methodName, method =>
                {
                    method.RepresentsModel(child); // throws exception because parent Class not set. Refactor CSharp builder to accomodate
                    if (methodName.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase) && operation.CallServiceOperationActionTargets().Any())
                    {
                        method.Async();
                    }

                    if (methodName is "OnInitializedAsync" or "OnInitialized")
                    {
                        method.Protected().Override();
                    }
                    foreach (var parameter in operation.Parameters)
                    {
                        method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                        {
                            if (parameter.Value != null)
                            {
                                param.WithDefaultValue(parameter.Value);
                            }
                        });
                    }

                    var mappingManager = template.CreateMappingManager();
                    mappingManager.SetFromReplacement(operation, null);

                    if (operation.CallServiceOperationActionTargets().Any())
                    {
                        var isLoadingProperty = template.BindingManager.GetElementBinding(componentElement.AsComponentModel().View, "2cfd43b2-2a18-4ac0-8cf3-d1aec9d7e699", isTargetNullable: false);
                        var errorMessageProperty = template.BindingManager.GetElementBinding(componentElement.AsComponentModel().View, "2e482e27-b176-43cf-b80a-33123036142a", isTargetNullable: false);
                        method.Async();
                        method.AddTryBlock(tryBlock =>
                        {
                            if (isLoadingProperty != null)
                            {
                                tryBlock.AddStatement(new CSharpAssignmentStatement(isLoadingProperty, "true"), s => s.WithSemicolon());
                            }
                            if (errorMessageProperty != null)
                            {
                                tryBlock.AddStatement(new CSharpAssignmentStatement(errorMessageProperty, "null"), s => s.WithSemicolon());
                            }

                            foreach (var serviceCall in operation.CallServiceOperationActionTargets())
                            {
                                var serviceName = ((IElement)serviceCall.Element).ParentElement.Name.ToPropertyName();
                                template.RazorFile.AddInjectDirective(template.GetTypeName(((IElement)serviceCall.Element).ParentElement), serviceName);
                                var invocation = mappingManager.GenerateUpdateStatements(serviceCall.GetMapInvocationMapping()).First();
                                if (serviceCall.GetMapResponseMapping() != null)
                                {
                                    if (serviceCall.GetMapResponseMapping().MappedEnds.Count == 1 && serviceCall.GetMapResponseMapping().MappedEnds.Single().SourceElement.Id == "28165dfb-a6a6-4c2b-9d64-421f1da81bc9")
                                    {
                                        tryBlock.AddStatement(new CSharpAssignmentStatement(
                                            lhs: mappingManager.GenerateTargetStatementForMapping(serviceCall.GetMapResponseMapping(), serviceCall.GetMapResponseMapping().MappedEnds.Single()),
                                            rhs: new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                    }
                                    else
                                    {
                                        tryBlock.AddStatement(new CSharpAssignmentStatement($"var {serviceCall.Name.ToLocalVariableName()}", new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                        mappingManager.SetFromReplacement(new StaticMetadata("28165dfb-a6a6-4c2b-9d64-421f1da81bc9"), serviceCall.Name.ToLocalVariableName());
                                        var response = mappingManager.GenerateUpdateStatements(serviceCall.GetMapResponseMapping());
                                        foreach (var statement in response)
                                        {
                                            statement.WithSemicolon();
                                        }

                                        tryBlock.AddStatements(response);
                                    }
                                }
                                else
                                {
                                    tryBlock.AddStatement(new CSharpAccessMemberStatement($"await {serviceName}", invocation));
                                }
                            }
                        });

                        method.AddCatchBlock(catchBlock =>
                        {
                            catchBlock.WithExceptionType("Exception").WithParameterName("e");
                            if (errorMessageProperty != null)
                            {
                                catchBlock.AddStatement(new CSharpAssignmentStatement(errorMessageProperty, "e.Message"), s => s.WithSemicolon());
                            }
                        });
                        method.AddFinallyBlock(finallyBlock =>
                        {
                            if (isLoadingProperty != null)
                            {
                                finallyBlock.AddStatement(new CSharpAssignmentStatement(isLoadingProperty, "false"), s => s.WithSemicolon());
                            }
                        });

                    }

                    foreach (var navigationModel in operation.NavigateToComponents())
                    {
                        method.Private();
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

                        template.RazorFile.AddInjectDirective("NavigationManager");
                        method.AddStatement($"NavigationManager.NavigateTo({(route.Route.Contains("{") ? $"${route.Route}" : route.Route)});");
                    }
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

        foreach (var associationEnd in componentElement.AssociatedElements)
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
                        method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
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

                    template.RazorFile.AddInjectDirective("NavigationManager");
                    method.AddStatement($"NavigationManager.NavigateTo({route});");
                });
            }
        }
    }

    public static void AddMappingReplacement(this IRazorFileNode node, IMetadataModel model, string replacementString)
    {
        var registry = node.HasMetadata("mapping-replacements")
            ? node.GetMetadata<IDictionary<IMetadataModel, string>>("mapping-replacements")
            : new Dictionary<IMetadataModel, string>();

        registry.TryAdd(model, replacementString);

        node.AddMetadata("mapping-replacements", registry);
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

    private static IEnumerable<IRazorFileNode> GetAllNodesInHierarchy(this IRazorFileNode node)
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
}

public record StaticMetadata(string id) : IMetadataModel
{
    public string Id { get; } = id;
}