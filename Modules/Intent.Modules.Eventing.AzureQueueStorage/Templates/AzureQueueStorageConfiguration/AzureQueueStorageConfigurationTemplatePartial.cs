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
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageOptions;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

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

                        mth.AddInvocationStatement($"services.AddOptions<{this.GetAzureQueueStorageOptionsName()}>", invoc =>
                        {
                            invoc.AddInvocation("Bind", bindInvoc => bindInvoc.AddArgument("configuration.GetSection(\"QueueStorage\")"));
                        });

                        foreach (var queueName in this.GetSubscribeQueues())
                        {
                            mth.AddInvocationStatement($"services.RegisterQueueStorageConsumers", invoc =>
                            {
                                invoc.AddArgument("configuration")
                                .AddArgument($"\"{queueName}\"");
                            });
                        }

                        mth.AddInvocationStatement($"services.Configure<{this.GetAzureQueueStorageSubscriptionOptionsName()}>", invoc =>
                        {
                            var subMessages = this.GetSubscribedMessages();
                            var subCommands = this.GetSubscribedIntegrationCommands();

                            if (subMessages.Count() + subCommands.Count() == 0)
                            {
                                invoc.AddArgument("options => {}");
                            }
                            else
                            {
                                var optionsLambda = new CSharpLambdaBlock("options");

                                foreach (var message in subMessages)
                                {
                                    optionsLambda.AddInvocationStatement($"options.Add<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, message)}, {this.GetIntegrationEventHandlerInterfaceName()}<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, message)}>>", invoc =>
                                    {
                                        invoc.AddArgument($"GetQueueName<{GetTypeName(IntegrationEventMessageTemplate.TemplateId, message)}>(configuration)");
                                    });
                                }

                                foreach (var command in subCommands)
                                {
                                    optionsLambda.AddInvocationStatement($"options.Add<{GetTypeName(IntegrationCommandTemplate.TemplateId, command)}, {this.GetIntegrationEventHandlerInterfaceName()}<{GetTypeName(IntegrationCommandTemplate.TemplateId, command)}>>", invoc =>
                                    {
                                        invoc.AddArgument($"GetQueueName<{GetTypeName(IntegrationCommandTemplate.TemplateId, command)}>(configuration)");
                                    });
                                }

                                invoc.AddArgument(optionsLambda);
                            }
                        });

                        if (GetSubscribedToMessageCount())
                        {
                            mth.AddStatement($"services.AddHostedService<{this.GetAzureQueueStorageConsumerBackgroundServiceName()}>();");
                        }

                        mth.AddReturn("services");
                    });

                    @class.AddMethod("string", "GetQueueName", mth =>
                    {
                        mth.Static().Private();
                        mth.AddGenericParameter("T")
                            .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        mth.AddReturn("configuration[$\"QueueStorage:PublishMap:{typeof(T).FullName}\"] ?? throw new ArgumentNullException($\"No type -> queue mapping for '{typeof(T).FullName}'\")");
                    });

                    @class.AddMethod("void", "RegisterQueueStorageConsumers", mth =>
                    {
                        mth.Static().Private();
                        mth.AddParameter(UseType("Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services", @param => param.WithThisModifier())
                            .AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration")
                            .AddParameter("string", "queueName");

                        AddTemplateDependency(AzureQueueStorageOptionsTemplate.TemplateId);
                        mth.AddObjectInitStatement("var queue", "new QueueDefinition();");
                        mth.AddInvocationStatement("configuration.GetSection", invoc =>
                        {
                            invoc.AddArgument("$\"QueueStorage:Queues:{queueName}\"");
                            invoc.AddInvocation("Bind", bindInvoc => bindInvoc.AddArgument("queue"));
                        });

                        mth.AddInvocationStatement("ArgumentNullException.ThrowIfNull", invoc => invoc.AddArgument("queue"));

                        mth.AddObjectInitStatement("queue.QueueName", "queueName;");
                        mth.AddObjectInitStatement("queue.Endpoint", "string.IsNullOrWhiteSpace(queue.Endpoint) ? configuration[\"QueueStorage:DefaultEndpoint\"] : queue.Endpoint;");

                        mth.AddInvocationStatement($"services.AddTransient<{this.GetAzureQueueStorageConsumerInterfaceName()}>", invoc =>
                        {
                            AddUsing("Microsoft.Extensions.DependencyInjection");

                            var ciInvoc = new CSharpInvocationStatement($"ActivatorUtilities.CreateInstance<{this.GetAzureQueueStorageConsumerName()}>")
                                .AddArgument("sp").AddArgument("queue").WithoutSemicolon();

                            invoc.AddArgument(new CSharpLambdaBlock("sp").WithExpressionBody(ciInvoc));
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

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("QueueStorage",
                new
                {
                    DefaultEndpoint = "UseDevelopmentStorage=true"
                }));

            var messages = Enumerable.Empty<MessageModel>()
                .Union(_messageModels.Value)
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, model),
                    QueueName: HelperExtensions.GetMessageQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            var commands = Enumerable.Empty<IntegrationCommandModel>()
                .Union(_integrationCommandModels.Value)
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, model),
                    QueueName: HelperExtensions.GetIntegrationCommandQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            foreach (var (fullyQualifiedTypeName, queueName) in messages.Union(commands))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:Queues:{queueName ?? fullyQualifiedTypeName}:Endpoint", null));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:Queues:{queueName ?? fullyQualifiedTypeName}:CreateQueue", false));
            }

            var publishedMessages = this.GetPublishedMessages()
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, model),
                    QueueName: HelperExtensions.GetMessageQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            var sentCommands = this.GetSentIntegrationCommands()
                .Select(model => (
                    FullyQualifiedTypeName: GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, model),
                    QueueName: HelperExtensions.GetIntegrationCommandQueue(model)))
                .OrderBy(x => x.FullyQualifiedTypeName);

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("QueueStorage:PublishMap",
               new
               {
               }));

            foreach (var (fullyQualifiedTypeName, queueName) in publishedMessages.Union(sentCommands).DistinctBy(d => d.FullyQualifiedTypeName))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"QueueStorage:PublishMap:{fullyQualifiedTypeName}", queueName));
            }
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