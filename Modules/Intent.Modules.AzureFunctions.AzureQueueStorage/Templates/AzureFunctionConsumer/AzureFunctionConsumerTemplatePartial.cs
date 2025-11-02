using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.UnitOfWork.Shared;
using Intent.Modules.Eventing.AzureQueueStorage;
using Intent.Modules.Eventing.AzureQueueStorage.Settings;
using Intent.Modules.Eventing.AzureQueueStorage.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureQueueStorage.Templates.AzureFunctionConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureFunctionConsumerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureQueueStorage.AzureFunctionConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionConsumerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsStorageQueues(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetRelativeLocation())
                .AddClass(GetFunctionName(), @class =>
                {
                    @class.AddField(UseType("System.Text.Json.JsonSerializerOptions"), "_serializerOptions", @field =>
                    {
                        @field.PrivateReadOnly();
                    });

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetAzureQueueStorageEventDispatcherInterfaceName(), "dispatcher", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<{@class.Name}>"), "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetEventBusInterfaceName(), "eventBus", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("System.IServiceProvider"), "serviceProvider", param => param.IntroduceReadonlyField());

                        ctor.AddObjectInitStatement("_serializerOptions", new CSharpObjectInitializerBlock("new JsonSerializerOptions")
                                .AddAssignmentStatement("PropertyNamingPolicy", new CSharpStatement("JsonNamingPolicy.CamelCase"))
                                .AddAssignmentStatement("PropertyNameCaseInsensitive", new CSharpStatement("true")).WithSemicolon());
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "Run", method =>
                    {
                        method.Async();
                        method.AddAttribute("Function", attr => attr.AddArgument($@"""{@class.Name}"""));
                        method.AddParameter(this.GetAzureQueueStorageEnvelopeName(), "message", param =>
                        {
                            param.AddAttribute(UseType("Microsoft.Azure.Functions.Worker.QueueTrigger"), attr =>
                            {
                                var messages = Model.IntegrationEventSubscriptions().Select(s => s.Element.AsMessageModel());
                                var commandModels = Model.IntegrationCommandSubscriptions().Select(s => s.Element.AsIntegrationCommandModel());

                                var queueName = messages is not null && messages.Any() ? HelperExtensions.GetMessageQueue(messages.First()) :
                                        commandModels is not null && commandModels.Any() ? HelperExtensions.GetIntegrationCommandQueue(commandModels.First()) :
                                        throw new InvalidOperationException($"Subscription could not be found for IntegrationEventHandler ['{Model.Id}', '{Model.Name}']");

                                attr.AddArgument($@"""{queueName}""");
                                attr.AddArgument(@"Connection = ""QueueStorage:DefaultEndpoint""");
                            });
                        });
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddTryBlock(block =>
                        {
                            var dispatch = new CSharpAwaitExpression(new CSharpInvocationStatement("_dispatcher", "DispatchAsync")
                                .AddArgument("_serviceProvider")
                                .AddArgument("message")
                                .AddArgument("_serializerOptions")
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
            if (ExecutionContext.Settings.GetAzureQueueStorageSettings().MessageEncoding().AsEnum() == AzureQueueStorageSettings.MessageEncodingOptionsEnum.None)
            {
                ExecutionContext.EventDispatcher.Publish(new HostSettingRegistrationRequest("extensions", new
                {
                    queues = new
                    {
                        messageEncoding = "none"
                    }
                }));
            }
        }
        //

        private string GetFunctionName()
        {
            var functionName = $"{Model.Name.RemoveSuffix("Handler")}Consumer";

            if (!ExecutionContext.Settings.GetAzureFunctionsSettings().SimpleFunctionNames())
            {
                var path = string.Join("_", Model.GetParentFolderNames());
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return $"{path}_{functionName}";
                }
            }

            return functionName;
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.GetParentFolderNames());
            return path;
        }

        string GetNamespace(params string[] additionalFolders)
        {
            return string.Join(".", new[]
                {
                    OutputTarget.GetNamespace()
                }
                .Concat(Model.GetParentFolders()
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