using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

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

    public static void AddCodeBlockMembers(this IRazorComponentClass block, IRazorComponentTemplate template, IElement componentElement)
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
                    if (child.AsPropertyModel().HasBindable())
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
                        var operationImplementationBlock = (IHasCSharpStatements)method;
                        var mappings = template.BindingManager.ViewBinding.MappedEnds.Where(x => x.SourceElement?.Id == operation.Id).ToList() ?? new();
                        var mappedButton = mappings.FirstOrDefault(x => x.TargetPath.Any(p => p.Element.SpecializationType == "Button"));
                        if (operation.CallServiceOperationActionTargets().Any())
                        {
                            var isLoadingProperty = template.BindingManager.GetElementBinding(componentElement.AsComponentModel().View, "2cfd43b2-2a18-4ac0-8cf3-d1aec9d7e699", isTargetNullable: false);
                            var errorMessageProperty = template.BindingManager.GetElementBinding(componentElement.AsComponentModel().View, "2e482e27-b176-43cf-b80a-33123036142a", isTargetNullable: false);
                            method.Async();
                            method.AddTryBlock(tryBlock =>
                            {
                                operationImplementationBlock = tryBlock;
                                if (isLoadingProperty != null)
                                {
                                    tryBlock.AddStatement(new CSharpAssignmentStatement(isLoadingProperty, "true"), s => s.WithSemicolon());
                                }
                                if (errorMessageProperty != null)
                                {
                                    tryBlock.AddStatement(new CSharpAssignmentStatement(errorMessageProperty, "null"), s => s.WithSemicolon());
                                }

                                //if (mappedButton != null)
                                //{
                                //    var processingFieldName = $"{operation.Name}Processing".ToPrivateMemberName();
                                //    block.AddField("bool", processingFieldName, f => f.WithAssignment("false"));
                                //    tryBlock.AddStatement(new CSharpAssignmentStatement(processingFieldName, "true"));
                                //}


                                foreach (var serviceCall in operation.CallServiceOperationActionTargets())
                                {
                                    var serviceName = ((IElement)serviceCall.Element).ParentElement.Name.ToPropertyName();
                                    block.AddInjectedProperty(block.Template.GetTypeName(((IElement)serviceCall.Element).ParentElement), serviceName);
                                    var invocation = mappingManager.GenerateUpdateStatements(serviceCall.GetMapInvocationMapping()).First();
                                    if (serviceCall.GetMapResponseMapping() != null)
                                    {
                                        var responseStaticElementId = "28165dfb-a6a6-4c2b-9d64-421f1da81bc9";
                                        if (serviceCall.GetMapResponseMapping().MappedEnds.Count == 1 && serviceCall.GetMapResponseMapping().MappedEnds.Single().SourceElement.Id == responseStaticElementId)
                                        {
                                            tryBlock.AddStatement(new CSharpAssignmentStatement(
                                                lhs: mappingManager.GenerateTargetStatementForMapping(serviceCall.GetMapResponseMapping(), serviceCall.GetMapResponseMapping().MappedEnds.Single()),
                                                rhs: new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                        }
                                        else
                                        {
                                            tryBlock.AddStatement(new CSharpAssignmentStatement($"var {serviceCall.Name.ToLocalVariableName()}", new CSharpAccessMemberStatement($"await {serviceName}", invocation)));
                                            mappingManager.SetFromReplacement(new StaticMetadata(responseStaticElementId), serviceCall.Name.ToLocalVariableName());
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

                                block.AddInjectedProperty("MudBlazor.ISnackbar", "Snackbar");
                                catchBlock.AddStatement($"Snackbar.Add(e.Message, {block.Template.UseType("MudBlazor.Severity")}.Error);");
                            });
                            method.AddFinallyBlock(finallyBlock =>
                            {
                                if (isLoadingProperty != null)
                                {
                                    finallyBlock.AddStatement(new CSharpAssignmentStatement(isLoadingProperty, "false"), s => s.WithSemicolon());
                                }
                                //if (mappings.Any(x => x.TargetPath.Any(p => p.Element.SpecializationType == "Button")))
                                //{
                                //    finallyBlock.AddStatement(new CSharpAssignmentStatement($"{operation.Name}Processing".ToPrivateMemberName(), "false"));
                                //}
                            });

                        }

                        foreach (var navigationModel in operation.NavigateToComponents())
                        {
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

                            block.AddInjectedProperty("Microsoft.AspNetCore.Components.NavigationManager");
                            operationImplementationBlock.AddStatement($"NavigationManager.NavigateTo({(route.Route.Contains("{") ? $"${route.Route}" : route.Route)});");
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

    public static string AddProcessingFieldName(this ICSharpClassMethod method)
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