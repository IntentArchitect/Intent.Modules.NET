using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.AzureQueueStorage.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageConfiguration";

        private readonly Lazy<IReadOnlyCollection<MessageModel>> _messageModels;
        private readonly Lazy<IReadOnlyCollection<IntegrationCommandModel>> _integrationCommandModels;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _messageModels = GetAllMessages();
            _integrationCommandModels = GetAllIntegrationCommands();

            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "AddQueueStorageConfiguration", mth =>
                    {
                        mth.Static();

                        mth.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", @param => param.WithThisModifier())
                           .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        mth.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureQueueStorageEventBusName()}>();");

                        mth.AddInvocationStatement($"services.Configure<{this.GetAzureQueueStorageOptionsName()}>", invoc =>
                        {
                            invoc.AddLambdaBlock("options", lambda =>
                            {
                                foreach (var message in _messageModels.Value)
                                {
                                    var messageTypeName = GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message);

                                    lambda.AddInvocationStatement($"AddQueue<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, message)}>", invoc1 =>
                                    {
                                        invoc1.AddArgument("configuration")
                                        .AddArgument("options")
                                        .AddArgument($"\"{messageTypeName}\"");
                                    });
                                }

                                foreach (var command in _integrationCommandModels.Value)
                                {
                                    var messageTypeName = GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command);

                                    lambda.AddInvocationStatement($"AddQueue<{GetTypeName(IntegrationCommandTemplate.TemplateId, command)}>", invoc1 =>
                                    {
                                        invoc1.AddArgument("configuration")
                                        .AddArgument("options")
                                        .AddArgument($"\"{messageTypeName}\"");
                                    });
                                }
                            });
                        });

                        mth.AddStatement($"services.AddScoped(typeof({this.GetAzureQueueStorageEventDispatcherInterfaceName()}<>), typeof({this.GetAzureQueueStorageEventDispatcherName()}<>));");
                        if (GetSubscribedToMessageCount())
                        {
                            mth.AddStatement($"services.AddHostedService<{this.GetAzureQueueStorageConsumerBackgroundServiceName()}>();");
                        }
                        foreach (var messageModel in ExecutionContext.MetadataManager.GetExplicitlySubscribedToMessageModels(outputTarget.Application))
                        {
                            mth.AddStatement($"services.AddTransient<{this.GetAzureQueueStorageConsumerInterfaceName()}, {this.GetAzureQueueStorageConsumerName()}<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, messageModel)}>>();");
                        }

                        foreach (var integrationCommandModel in ExecutionContext.MetadataManager.GetExplicitlySubscribedToIntegrationCommandModels(outputTarget.Application))
                        {
                            mth.AddStatement($"services.AddTransient<{this.GetAzureQueueStorageConsumerInterfaceName()}, {this.GetAzureQueueStorageConsumerName()}<{GetTypeName(IntegrationCommandTemplate.TemplateId, integrationCommandModel)}>>();");
                        }

                        mth.AddReturn("services");
                    });

                    @class.AddMethod("void", "AddQueue", mth =>
                    {
                        mth.Static().Private();
                        mth.AddGenericParameter("T");

                        mth.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration")
                        .AddParameter(this.GetAzureQueueStorageOptionsName(), "options")
                        .AddParameter("string", "messageType");

                        var createQueueParseInvoc = new CSharpInvocationStatement("bool.TryParse")
                            .AddArgument("configuration[$\"QueueStorage:Queues:{messageType}:CreateQueue\"]")
                            .AddArgument("out bool createQueue");
                        var maxMessagesParseInvoc = new CSharpInvocationStatement("int.TryParse")
                            .AddArgument("configuration[$\"QueueStorage:Queues:{messageType}:MaxMessages\"]")
                            .AddArgument("out int maxMessages");

                        mth.AddObjectInitStatement("_", createQueueParseInvoc);
                        mth.AddObjectInitStatement("_", maxMessagesParseInvoc);

                        mth.AddInvocationStatement("options.AddQueue<T>", invoc =>
                        {
                            invoc.AddArgument("configuration[$\"QueueStorage:Queues:{messageType}:Endpoint\"]!")
                            .AddArgument("configuration[$\"QueueStorage:Queues:{messageType}:QueueName\"]!")
                            .AddArgument("createQueue")
                            .AddArgument("maxMessages");
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddQueueStorageConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("QueueStorage:Queues",
                new
                {
                }));

            var messages = Enumerable.Empty<MessageModel>()
                .Union(_messageModels.Value)
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, model),
                    QueueName: GetMessageQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            var commands = Enumerable.Empty<IntegrationCommandModel>()
                .Union(_integrationCommandModels.Value)
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, model),
                    QueueName: GetIntegrationCommandQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            foreach (var (fullyQualifiedTypeName, queue) in messages.Union(commands))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:Queues:{fullyQualifiedTypeName}:QueueName", queue));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:Queues:{fullyQualifiedTypeName}:Endpoint", string.Empty));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:Queues:{fullyQualifiedTypeName}:CreateQueue", false));
            }
        }

        private static string GetMessageQueue(MessageModel messageModel)
        {
            var stack = new Stack<string>();
            var element = messageModel.InternalElement;

            if (messageModel.HasAzureQueueStorage() && !string.IsNullOrWhiteSpace(messageModel.GetAzureQueueStorage().QueueName()))
            {
                return messageModel.GetAzureQueueStorage().QueueName();
            }

            while (true)
            {
                stack.Push(element.Name.ToLower());

                if (element.ParentElement == null)
                {
                    stack.Push(element.Package.Name.ToLower());
                    break;
                }

                element = element.ParentElement;
            }

            return string.Join('-', stack).ToLower().Replace('.', '-');
        }

        private static string GetIntegrationCommandQueue(IntegrationCommandModel commandMode)
        {
            var stack = new Stack<string>();
            var element = commandMode.InternalElement;

            if (commandMode.HasAzureQueueStorage() && !string.IsNullOrWhiteSpace(commandMode.GetAzureQueueStorage().QueueName()))
            {
                return commandMode.GetAzureQueueStorage().QueueName();
            }

            while (true)
            {
                stack.Push(element.Name.ToLower());

                if (element.ParentElement == null)
                {
                    stack.Push(element.Package.Name.ToLower());
                    break;
                }

                element = element.ParentElement;
            }

            return string.Join('-', stack).ToLower().Replace('.', '-');
        }

        private bool GetSubscribedToMessageCount()
        {
            return (ExecutionContext.MetadataManager
                        .GetExplicitlySubscribedToMessageModels(OutputTarget.Application).Count +
                    ExecutionContext.MetadataManager
                        .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                        .SelectMany(x => x.SubscribedMessages())
                        .Select(x => x.TypeReference.Element.AsMessageModel()).Count() +
                    ExecutionContext.MetadataManager
                        .GetExplicitlySubscribedToIntegrationCommandModels(OutputTarget.Application).Count) > 0;
        }

        private Lazy<IReadOnlyCollection<MessageModel>> GetAllMessages()
        {
            return new Lazy<IReadOnlyCollection<MessageModel>>(() =>
            {
                var serviceDesignerSubscribeMessages = ExecutionContext.MetadataManager
                    .GetExplicitlySubscribedToMessageModels(OutputTarget.Application);

                var serviceDesignerPublishedMessages = ExecutionContext.MetadataManager
                    .GetExplicitlyPublishedMessageModels(OutputTarget.Application);

                var eventingDesignerSubscribedMessages = ExecutionContext.MetadataManager
                    .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                    .SelectMany(x => x.SubscribedMessages())
                    .Select(x => x.TypeReference.Element.AsMessageModel());

                var eventingDesignerPublishedMessages = ExecutionContext.MetadataManager
                    .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                    .SelectMany(x => x.PublishedMessages())
                    .Select(x => x.TypeReference.Element.AsMessageModel());

                var messageModels = Enumerable.Empty<MessageModel>()
                    .Concat(serviceDesignerSubscribeMessages)
                    .Concat(serviceDesignerPublishedMessages)
                    .Concat(eventingDesignerSubscribedMessages)
                    .Concat(eventingDesignerPublishedMessages)
                    .OrderBy(x => x.Name)
                    .Distinct()
                    .ToArray();

                return messageModels;
            });
        }

        private Lazy<IReadOnlyCollection<IntegrationCommandModel>> GetAllIntegrationCommands()
        {
            return new Lazy<IReadOnlyCollection<IntegrationCommandModel>>(() =>
            {
                var serviceDesignerSubscribeCommands = ExecutionContext.MetadataManager
                    .GetExplicitlySubscribedToIntegrationCommandModels(OutputTarget.Application);

                var serviceDesignerSendCommands = ExecutionContext.MetadataManager
                    .GetExplicitlySentIntegrationCommandModels(OutputTarget.Application);

                var messageModels = Enumerable.Empty<IntegrationCommandModel>()
                    .Concat(serviceDesignerSubscribeCommands)
                    .Concat(serviceDesignerSendCommands)
                    .OrderBy(x => x.Name)
                    .Distinct()
                    .ToArray();

                return messageModels;
            });
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