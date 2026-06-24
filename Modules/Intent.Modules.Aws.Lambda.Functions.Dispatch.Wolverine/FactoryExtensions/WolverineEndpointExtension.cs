using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates.ApplicationHandlerPolicy;
using Intent.Modules.Application.Wolverine.Templates.CommandHandler;
using Intent.Modules.Application.Wolverine.Templates.CommandModels;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Application.Wolverine.Templates.QueryModels;
using Intent.Modules.Application.Wolverine.Templates.WolverineConfiguration;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Templates;
using Intent.Modules.Aws.Lambda.Functions.Dispatch.Wolverine.Templates.LambdaFunction;
using Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;
using Intent.Modules.Aws.Lambda.Functions.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineEndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.Lambda.Functions.Dispatch.Wolverine.WolverineEndpointExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<LambdaFunctionClassTemplate>(LambdaFunctionClassTemplate.TemplateId);
            foreach (var containerTemplate in templates)
            {
                if (containerTemplate.Model is not CqrsLambdaFunctionContainerModel)
                {
                    continue;
                }

                containerTemplate.AddTypeSource(CommandModelsTemplate.TemplateId);
                containerTemplate.AddTypeSource(QueryModelsTemplate.TemplateId);
                containerTemplate.AddTypeSource(ClassTypeSource.Create(application, "Application.Contract.Dto")
                    .WithCollectionFormatter(CSharpCollectionFormatter.CreateList()));
                containerTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(containerTemplate.UseType("Wolverine.IMessageBus"), "sender",
                        p => p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()));

                    foreach (var actionMethod in @class.Methods)
                    {
                        var operationModel = (ILambdaFunctionModel)actionMethod.RepresentedModel;

                        containerTemplate.CSharpFile.OnBuild(_ =>
                        {
                            actionMethod.InsertStatement(0, $"""
                                                             // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
                                                             var cancellationToken = {containerTemplate.UseType("System.Threading.CancellationToken")}.None;
                                                             """);
                        }, int.MaxValue);

                        AddCqrsParameterToFieldAssignments(application, actionMethod, operationModel);
                        actionMethod.AddStatements(GetValidations(containerTemplate, operationModel));
                        actionMethod.AddStatement(GetDispatchViaWolverineStatement(containerTemplate, operationModel), s => s.SeparatedFromPrevious());

                        var returnStatement = actionMethod.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                        if (returnStatement != null)
                        {
                            returnStatement.Remove();
                            actionMethod.AddStatement(containerTemplate.GetReturnStatement(operationModel));
                        }
                    }
                }, 10);
            }

            RegisterWolverineOnLambdaStartup(application.FindTemplateInstance<StartupTemplate>(StartupTemplate.TemplateId));
            ApplyServerlessDiscovery(application);
        }

        private static void RegisterWolverineOnLambdaStartup(StartupTemplate startupTemplate)
        {
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(Intent.Modules.Application.Wolverine.NugetPackages.WolverineFx(startupTemplate.OutputTarget));

            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Wolverine");

                var wolverineConfigType = startupTemplate.GetTypeName(WolverineConfigurationTemplate.TemplateId);
                var configureMethod = file.Classes.First().FindMethod("ConfigureHostBuilder");
                var returnStatement = configureMethod.FindStatement(s => s.GetText(string.Empty).TrimStart().StartsWith("return "));

                var statement = $"hostBuilder.UseWolverine(opts => {{ {wolverineConfigType}.Configure(opts); }});";

                if (returnStatement != null)
                {
                    returnStatement.InsertAbove(statement);
                }
                else
                {
                    configureMethod.AddStatement(statement);
                }
            }, 500);
        }

        private static void ApplyServerlessDiscovery(IApplication application)
        {
            var wolverineConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineConfigurationTemplate.TemplateId);
            if (wolverineConfigTemplate == null)
            {
                return;
            }

            var commandHandlerTemplates = application.FindTemplateInstances<IIntentTemplate<CommandModel>>(CommandHandlerTemplate.TemplateId);
            var queryHandlerTemplates = application.FindTemplateInstances<IIntentTemplate<QueryModel>>(QueryHandlerTemplate.TemplateId);

            wolverineConfigTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("JasperFx.CodeGeneration");

                var configureMethod = file.Classes.First().FindMethod("Configure");
                configureMethod.Statements.Clear();

                configureMethod.AddStatement("opts.Discovery.DisableConventionalDiscovery();");
                configureMethod.AddStatement("");

                foreach (var t in commandHandlerTemplates)
                {
                    var handlerType = wolverineConfigTemplate.GetTypeName(CommandHandlerTemplate.TemplateId, t.Model);
                    configureMethod.AddStatement($"opts.Discovery.IncludeType<{handlerType}>();");
                }

                foreach (var t in queryHandlerTemplates)
                {
                    var handlerType = wolverineConfigTemplate.GetTypeName(QueryHandlerTemplate.TemplateId, t.Model);
                    configureMethod.AddStatement($"opts.Discovery.IncludeType<{handlerType}>();");
                }

                configureMethod.AddStatement("");
                configureMethod.AddStatement("opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;");
                configureMethod.AddStatement("opts.Durability.Mode = DurabilityMode.Serverless;");
                configureMethod.AddStatement("");

                var handlerPolicyType = wolverineConfigTemplate.GetTypeName(ApplicationHandlerPolicyTemplate.TemplateId);
                configureMethod.AddStatement($"{handlerPolicyType}.Apply(opts);");
            }, 500);
        }

        private static void AddCqrsParameterToFieldAssignments(
            IApplication application,
            CSharpClassMethod actionMethod,
            ILambdaFunctionModel operationModel)
        {
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter == null)
            {
                return;
            }

            var commandModelTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(CommandModelsTemplate.TemplateId, operationModel.InternalElement.Id);
            if (commandModelTemplate is not null)
            {
                commandModelTemplate.CSharpFile.OnBuild(file =>
                {
                    var statements = GetGenericParameterToFieldStatements(actionMethod, commandModelTemplate, payloadParameter);
                    var index = -1;
                    foreach (var statement in statements)
                    {
                        index++;
                        actionMethod.InsertStatement(index, statement);
                    }
                }, 10);
            }

            var queryModelTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(QueryModelsTemplate.TemplateId, operationModel.InternalElement.Id);
            if (queryModelTemplate is not null)
            {
                queryModelTemplate.CSharpFile.OnBuild(file =>
                {
                    var statements = GetGenericParameterToFieldStatements(actionMethod, queryModelTemplate, payloadParameter);
                    var index = -1;
                    foreach (var statement in statements)
                    {
                        index++;
                        actionMethod.InsertStatement(index, statement);
                    }
                }, 10);
            }
        }

        private static IReadOnlyCollection<CSharpStatement> GetGenericParameterToFieldStatements(
            CSharpClassMethod actionMethod,
            ICSharpFileBuilderTemplate cqrsModelTemplate,
            IEndpointParameterModel payloadParameter)
        {
            var statements = new List<CSharpStatement>();
            var commandClass = cqrsModelTemplate.CSharpFile.Classes.First();

            foreach (var methodParameter in actionMethod.Parameters)
            {
                var paramModelId = ((IEndpointParameterModel)methodParameter.RepresentedModel).Id;

                var commandProp = commandClass.Properties.FirstOrDefault(prop =>
                    (prop.RepresentedModel as IEndpointParameterModel)?.Id == paramModelId);
                if (commandProp is null)
                {
                    continue;
                }

                statements.Add(new CSharpIfStatement($"{payloadParameter.Name}.{commandProp.Name} == {GetComparisonValue(commandProp.Type?.ToTypeName() ?? string.Empty)}")
                    .AddStatement($"{payloadParameter.Name}.{commandProp.Name} = {methodParameter.Name};"));
            }

            return statements;
        }

        private static string GetComparisonValue(string type) => type switch
        {
            "Guid" => "Guid.Empty",
            _ => "default"
        };

        private IEnumerable<CSharpStatement> GetValidations(LambdaFunctionClassTemplate containerTemplate, ILambdaFunctionModel operationModel)
        {
            var validations = new List<CSharpStatement>();
            var payloadParameter = GetPayloadParameter(operationModel);
            if (payloadParameter == null)
            {
                return validations;
            }

            foreach (var mappedParameter in GetMappedParameters(operationModel))
            {
                validations.Add(new CSharpIfStatement(
                        $"{(mappedParameter.TypeReference.HasGuidType() ? $"{mappedParameter.Name.ToParameterName()}Guid" : mappedParameter.Name.ToParameterName())} != {payloadParameter.Name}.{mappedParameter.MappedPayloadProperty!.Name.ToPascalCase()}")
                    .AddStatement($"return {containerTemplate.UseType("Amazon.Lambda.Annotations.APIGateway.HttpResults")}.BadRequest();"));
            }

            return validations;
        }

        private CSharpStatement GetDispatchViaWolverineStatement(LambdaFunctionClassTemplate template, ILambdaFunctionModel operationModel)
        {
            var payload = GetPayloadParameter(operationModel)?.Name
                          ?? GetMappedPayload(template, operationModel);

            var statementRaw = operationModel.ReturnType != null
                ? $"var result = await _sender.InvokeAsync<{template.GetTypeName(operationModel.ReturnType)}>({payload}, cancellationToken);"
                : $"await _sender.InvokeAsync({payload}, cancellationToken);";

            return new CSharpStatement(statementRaw).AddMetadata("dispatch-command", "wolverine");
        }

        private static IEndpointParameterModel? GetPayloadParameter(ILambdaFunctionModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.TypeReference.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.TypeReference.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(LambdaFunctionClassTemplate template, ILambdaFunctionModel operationModel)
        {
            var requestType = operationModel.InternalElement.IsCommandModel() || operationModel.InternalElement.IsQueryModel()
                ? operationModel.InternalElement.AsTypeReference()
                : operationModel.InternalElement.MappedElement;

            if (GetMappedParameters(operationModel).Any())
            {
                return $"new {template.GetTypeName(requestType)} ( {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.MappedPayloadProperty!.TypeReference.HasGuidType() ? $"{x.MappedPayloadProperty.Name.ToParameterName()}Guid" : x.MappedPayloadProperty.Name.ToParameterName() + " : " + x.Name))} )";
            }

            return $"new {template.GetTypeName(requestType)}()";
        }

        private IList<IEndpointParameterModel> GetMappedParameters(ILambdaFunctionModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.MappedPayloadProperty != null).ToList();
        }
    }
}
