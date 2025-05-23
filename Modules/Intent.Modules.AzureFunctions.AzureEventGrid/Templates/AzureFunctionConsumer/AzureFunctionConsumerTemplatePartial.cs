using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.AzureEventGrid.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureEventGrid.Templates.AzureFunctionConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureFunctionConsumerTemplate : CSharpTemplateBase<AzureFunctionSubscriptionModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureEventGrid.AzureFunctionConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionConsumerTemplate(IOutputTarget outputTarget, AzureFunctionSubscriptionModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsEventGrid(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetRelativeLocation())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass(GetFunctionName(), @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetAzureEventGridMessageDispatcherInterfaceName(), "dispatcher", param => param.IntroduceReadonlyField());
                        ctor.AddParameter($"ILogger<{@class.Name}>", "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetEventBusInterfaceName(), "eventBus", param => param.IntroduceReadonlyField());
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Task", "Run", method =>
                    {
                        method.Async();
                        method.AddAttribute("Function", attr => attr.AddArgument($@"""{@class.Name}"""));
                        method.AddParameter(UseType("Azure.Messaging.EventGrid.EventGridEvent"), "message", param =>
                        {
                            param.AddAttribute(UseType("Microsoft.Azure.Functions.Worker.EventGridTrigger"));
                        });
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddTryBlock(block =>
                        {
                            var dispatch = new CSharpAwaitExpression(new CSharpInvocationStatement("_dispatcher", "DispatchAsync")
                                .AddArgument("_serviceProvider")
                                .AddArgument("message")
                                .AddArgument("cancellationToken"));
                            dispatch.AddMetadata("service-dispatch-statement", true);

                            block.ApplyUnitOfWorkImplementations(this, @class.Constructors.First(), dispatch);

                            block.AddStatement($@"await _eventBus.FlushAllAsync(cancellationToken);");
                        });
                        method.AddCatchBlock(UseType("System.Exception"), "ex", block =>
                        {
                            block.AddStatement($@"_logger.LogError(ex, ""Error processing {@class.Name}"");");
                            block.AddStatement("throw;");
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            var messageNames = Model.MessageModels
                .Select(x => GetFullyQualifiedTypeName(GetTemplate<IClassProvider>(
                    templateId: "Application.Eventing.Message",
                    modelId: x.Id,
                    options: new TemplateDiscoveryOptions { TrackDependency = false })))
                .ToArray();

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(
                infrastructureComponent: Infrastructure.AzureEventGrid.Subscription,
                properties: new Dictionary<string, string>
                {
                    [Infrastructure.AzureEventGrid.Property.MessageNames] = string.Join(";", messageNames),
                    [Infrastructure.AzureEventGrid.Property.HandlerFunctionName] = GetFunctionName(),
                    [Infrastructure.AzureEventGrid.Property.TopicName] = Model.TopicName
                }));

            base.AfterTemplateRegistration();
        }

        private string GetFunctionName()
        {
            var functionName = $"{Model.HandlerModel.Name.RemoveSuffix("Handler")}Consumer";

            if (!ExecutionContext.Settings.GetAzureFunctionsSettings().SimpleFunctionNames())
            {
                var path = string.Join("_", Model.HandlerModel.GetParentFolderNames());
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return $"{path}_{functionName}";
                }
            }

            return functionName;
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.HandlerModel.GetParentFolderNames());
            return path;
        }

        string GetNamespace(params string[] additionalFolders)
        {
            return string.Join(".", new[]
                {
                    OutputTarget.GetNamespace()
                }
                .Concat(Model.HandlerModel.GetParentFolders()
                    .Where(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.Name))
                            return false;

                        if (x is FolderModel fm)
                        {
                            return !fm.HasFolderOptions() || fm.GetFolderOptions().NamespaceProvider();
                        }

                        return true;
                    })
                    .Select(x => x.Name))
                .Concat(additionalFolders));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}