using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Intent.AspNetCore.Mvc.Api;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.AspNetCore.Mvc.Templates;
using Intent.Modules.AspNetCore.Mvc.Templates.MvcController;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Shared;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Mvc.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MvcControllerDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Mvc.MvcControllerDispatchExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<MvcControllerTemplate>(TemplateDependency.OnTemplate(MvcControllerTemplate.TemplateId));

            foreach (var template in templates)
            {
                InstallServiceContractDispatch(template);
                InstallValidation(template);
                InstallTransactionWithUnitOfWork(template, application);
                InstallMessageBus(template);
            }

            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            startupTemplate.CSharpFile.OnBuild(_ =>
            {
                startupTemplate.StartupFile.ConfigureServices((statements, context) =>
                {
                    var original = statements.Statements
                        .OfType<CSharpInvocationStatement>()
                        .Single(x =>
                            x.TryGetMetadata<string>("lambda-registration-for", out var value) &&
                            value == "AddControllers");

                    var replaceWith = new CSharpInvocationStatement($"{context.Services}.AddControllersWithViews");

                    foreach (var (key, value) in original.Metadata)
                    {
                        replaceWith.AddMetadata(key, value);
                    }

                    foreach (var statement in original.Statements)
                    {
                        replaceWith.AddStatement(statement);
                    }

                    original.Replace(replaceWith);
                });

                startupTemplate.StartupFile.ConfigureEndpoints((statements, context) =>
                {
                    statements.AddStatement(new CSharpInvocationStatement($"{context.Endpoints}.MapControllerRoute")
                        .AddArgument("name", "\"default\"")
                        .AddArgument("pattern", "\"{controller=Home}/{action=Index}/{id?}\""));
                });
            });
        }

        private static void InstallValidation(MvcControllerTemplate template)
        {
            if (!template.TryGetTypeName(TemplateRoles.Application.Common.ValidationServiceInterface, out var validationProviderName))
            {
                return;
            }

            if (template.Model.Operations.All(o => o.Parameters.All(x => !template.TryGetTypeName(TemplateRoles.Application.Validation.Dto, x.TypeReference.Element, out _))))
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                @class.Constructors.First().AddParameter(validationProviderName, "validationService", param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));

                foreach (var method in @class.Methods)
                {
                    var fromBodyParam = method.Parameters.FirstOrDefault(p =>
                        p.Attributes.Any(static parameter => parameter.GetText("")?.Contains("FromBody") == true));
                    if (fromBodyParam != null)
                    {
                        method.InsertStatement(0, $"await _validationService.Handle({fromBodyParam.Name}, cancellationToken);");
                    }
                }
            });
        }

        private void InstallServiceContractDispatch(MvcControllerTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model), "appService", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (!method.TryGetMetadata<OperationModel>("model", out var operationModel) ||
                        !operationModel.TryGetMVCSettings(out var mvcSettings))
                    {
                        continue;
                    }

                    var awaitModifier = string.Empty;
                    var arguments = string.Join(", ", operationModel.Parameters.Select((x) => x.Name ?? ""));

                    if (!operationModel.InternalElement.HasStereotype("Synchronous"))
                    {
                        awaitModifier = "await ";
                        arguments = string.IsNullOrEmpty(arguments)
                            ? "cancellationToken"
                            : $"{arguments}, cancellationToken";
                    }

                    if (operationModel.ReturnType != null)
                    {
                        var defaultResultValue = GetDefaultValue(template.GetTypeName(operationModel));

                        method.AddStatement($"var result = {defaultResultValue};");
                        method.AddStatement($"result = {awaitModifier}_appService.{operationModel.Name.ToPascalCase()}({arguments});",
                            stmt => stmt.AddMetadata("service-contract-dispatch", true));
                    }
                    else
                    {
                        method.AddStatement($"{awaitModifier}_appService.{operationModel.Name.ToPascalCase()}({arguments});",
                            stmt => stmt.AddMetadata("service-contract-dispatch", true));
                    }

                    switch (mvcSettings.ReturnType().AsEnum())
                    {
                        case OperationModelStereotypeExtensions.MVCSettings.ReturnTypeOptionsEnum.OperationReturnType:
                            method.AddStatement($"return result;", s => s.SeparatedFromPrevious());
                            break;
                        case OperationModelStereotypeExtensions.MVCSettings.ReturnTypeOptionsEnum.Ok:
                            method.AddStatement($"return Ok();", s => s.SeparatedFromPrevious());
                            break;
                        case OperationModelStereotypeExtensions.MVCSettings.ReturnTypeOptionsEnum.RedirectToAction:
                            var redirectToArguments = new List<string>(4) {
                                // actionName
                                $"\"{mvcSettings.RedirectToAction().Name.ToPascalCase()}\""
                            };

                            if (mvcSettings.RedirectToController() != null)
                            {
                                var controllerName = template.GetMvcControllerName(mvcSettings.RedirectToController().AsServiceModel()).RemoveSuffix("Controller");
                                redirectToArguments.Add($"\"{controllerName}\"");
                            }

                            if (!string.IsNullOrWhiteSpace(mvcSettings.RedirectToRouteValues()))
                            {
                                redirectToArguments.Add(mvcSettings.RedirectToRouteValues());
                            }
                            else if (TryGetRedirectToParameterObject(operationModel, mvcSettings, out var item))
                            {
                                redirectToArguments.Add(item);
                            }

                            if (!string.IsNullOrWhiteSpace(mvcSettings.RedirectToFragment()))
                            {
                                redirectToArguments.Add($"\"{mvcSettings.RedirectToFragment()}\"");
                            }

                            method.AddStatement($"return RedirectToAction({string.Join(", ", redirectToArguments)});", s => s.SeparatedFromPrevious());

                            break;
                        case OperationModelStereotypeExtensions.MVCSettings.ReturnTypeOptionsEnum.View:
                            var viewArguments = new List<string>();

                            if (!string.IsNullOrWhiteSpace(mvcSettings.ViewName()))
                            {
                                viewArguments.Add("\"view\"");
                            }

                            if (operationModel.ReturnType != null)
                            {
                                viewArguments.Add("result");
                            }

                            method.AddStatement($"return View({string.Join(", ", viewArguments)});", s => s.SeparatedFromPrevious());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }

        private bool TryGetRedirectToParameterObject(
            OperationModel sourceOperation,
            OperationModelStereotypeExtensions.MVCSettings settings,
            out string result)
        {
            var targetOperation = settings.RedirectToAction().AsOperationModel();

            var methodRouteParameters = Regex.Matches(targetOperation.GetMVCSettings()?.Route() ?? string.Empty, "{({*[^{}]*}*)}")
                .Select(x =>
                {
                    var value = x.Groups[1].Value;
                    var targetParameter = targetOperation.Parameters.SingleOrDefault(y => string.Equals(y.Name, value, StringComparison.OrdinalIgnoreCase));
                    var sourceParameter = sourceOperation.Parameters.SingleOrDefault(y => string.Equals(y.Name, value, StringComparison.OrdinalIgnoreCase));

                    return new
                    {
                        IsControllerRoute = false,
                        Name = value,
                        TargetParameter = targetParameter,
                        SourceParameter = sourceParameter,
                    };
                })
                .ToArray();
            var controllerRouteParameters = Regex.Matches(targetOperation.ParentService.GetMVCSettings()?.Route() ?? string.Empty, "{({*[^{}]*}*)}")
                .Select(x =>
                {
                    var value = x.Groups[1].Value;
                    var targetParameter = targetOperation.Parameters.SingleOrDefault(y => string.Equals(y.Name, value, StringComparison.OrdinalIgnoreCase));
                    var sourceParameter = sourceOperation.Parameters.SingleOrDefault(y => string.Equals(y.Name, value, StringComparison.OrdinalIgnoreCase));

                    return new
                    {
                        IsControllerRoute = true,
                        Name = value,
                        TargetParameter = targetParameter,
                        SourceParameter = sourceParameter,
                    };
                })
                .ToArray();

            if (methodRouteParameters.Length == 0 &&
                controllerRouteParameters.Length == 0)
            {
                result = default;
                return false;
            }

            var sb = new StringBuilder();
            sb.Append("new {");

            foreach (var item in methodRouteParameters.Concat(controllerRouteParameters))
            {
                if (item.SourceParameter != null &&
                    item.TargetParameter != null)
                {
                    var propertyName = item.Name == item.SourceParameter.Name.ToCamelCase()
                        ? string.Empty
                        : $"{item.Name.ToCamelCase()} = ";

                    sb.Append($" {propertyName}{item.Name},");
                    continue;
                }

                if (targetOperation.ReturnType != null &&
                    item.SourceParameter == null)
                {
                    sb.Append($" {item.Name} = result,");
                    continue;
                }

                throw new ElementException(
                    sourceOperation.InternalElement,
                    $"Could not match source parameter \"{{{item.Name}}}\" with route parameter on RedirectToAction destination");
            }

            // Remove trailing comma
            sb.Length -= 1;

            sb.Append(" }");

            result = sb.ToString();
            return true;
        }

        private static string GetDefaultValue(string type) => type switch
        {
            "Guid" => "Guid.Empty",
            _ => $"default({type})"
        };

        private static void InstallTransactionWithUnitOfWork(MvcControllerTemplate template, IApplication application)
        {
            if (!template.SystemUsesPersistenceUnitOfWork())
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                foreach (var method in @class.Methods)
                {
                    if (!method.TryGetMetadata<OperationModel>("model", out var operation) ||
                        !operation.TryGetMVCSettings(out var mvcSettings) ||
                        mvcSettings.Verb().IsGET())
                    {
                        continue;
                    }

                    var dispatchStmt = method.FindStatement(stmt => stmt.HasMetadata("service-contract-dispatch"));
                    if (dispatchStmt == null)
                    {
                        continue;
                    }

                    //remove current dispatch statement (UOW implementation replaces it)
                    dispatchStmt.Remove();
                    method.ApplyUnitOfWorkImplementations(
                        template: template,
                        constructor: @class.Constructors.First(),
                        invocationStatement: (CSharpStatement)dispatchStmt,
                        returnType: null,
                        resultVariableName: "result",
                        fieldSuffix: "unitOfWork",
                        includeComments: false);

                    //Move return statement to the end
                    var returnStatement = method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                    if (returnStatement != null)
                    {
                        returnStatement.Remove();
                        method.AddStatement(returnStatement);
                    }

                }
            }, order: 1);
        }

        private static void InstallMessageBus(MvcControllerTemplate template)
        {
            var shouldInstallMessageBus = TryGetMessageBusInterfaceName(template, out var messageBusInterfaceName);
            if (!shouldInstallMessageBus)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(messageBusInterfaceName, "messageBus",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                foreach (var method in @class.Methods.Where(x => x.Attributes.All(a => !a.ToString()!.StartsWith("[HttpGet"))))
                {
                    method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "))?
                        .InsertAbove("await _messageBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
                }
            }, order: -100);
        }
        
        private static bool TryGetMessageBusInterfaceName(IIntentTemplate template, out string typeName)
        {
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out typeName))
            {
                return true;
            }

            // Legacy support
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out typeName))
            {
                return true;
            }

            typeName = null;
            return false;
        }
    }
}