using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    public static class RazorFileExtensions
    {
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
                    block.AddMethod(template.GetTypeName(operation.TypeReference), operation.Name.ToPropertyName(), method =>
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
                            method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
                            {
                                if (parameter.Value != null)
                                {
                                    param.WithDefaultValue(parameter.Value);
                                }
                            });
                        }

                        var mappingManager = template.CreateMappingManager();

                        foreach (var serviceCall in operation.CallServiceOperationActionTargets())
                        {
                            method.Async();
                            var serviceName = ((IElement)serviceCall.Element).ParentElement.Name.ToPropertyName();
                            template.AddInjectDirective(template.GetTypeName(((IElement)serviceCall.Element).ParentElement), serviceName);
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
                                method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToParameterName(), param =>
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

                            template.AddInjectDirective("NavigationManager");
                            method.AddStatement($"NavigationManager.NavigateTo({route});");
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

                        template.AddInjectDirective("NavigationManager");
                        method.AddStatement($"NavigationManager.NavigateTo({route});");
                    });
                }
            }
        }
    }
}