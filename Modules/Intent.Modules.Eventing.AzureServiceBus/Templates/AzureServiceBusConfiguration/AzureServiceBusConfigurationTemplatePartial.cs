using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureServiceBusConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureServiceBusConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"AzureServiceBusConfiguration", @class =>
                {
                    @class.Static()
                        .AddMethod("IServiceCollection", "ConfigureAzureServiceBus", method =>
                        {
                            method.Static()
                                .AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                                .AddParameter("IConfiguration", "configuration");

                            method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureServiceBusEventBusName()}>();")
                                .AddStatement($"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherName()}>();")
                                .AddStatement(
                                    $"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherInterfaceName()}, {this.GetAzureServiceBusMessageDispatcherName()}>();");

                            var sendCommands = GetCommandsBeingSent();
                            var publishMessages = GetMessagesBeingPublished();
                            if (publishMessages.Count != 0 || sendCommands.Count != 0)
                            {
                                method.AddInvocationStatement($"services.Configure<{this.GetPublisherOptionsName()}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var sendCommand in sendCommands)
                                        {
                                            arg.AddStatement(
                                                $"""options.Add<{this.GetIntegrationCommandName(sendCommand)}>(configuration["AzureServiceBus:{sendCommand.GetCommandQueueOrTopicConfigurationName()}"]!);""");
                                        }

                                        foreach (var publishMessage in publishMessages)
                                        {
                                            arg.AddStatement(
                                                $"""options.Add<{this.GetIntegrationEventMessageName(publishMessage)}>(configuration["AzureServiceBus:{publishMessage.GetMessageQueueOrTopicConfigurationName()}"]!);""");
                                        }
                                    }));
                            }

                            var eventHandlers = GetEventMessagesBeingSubscribed();
                            var commandHandlers = GetCommandMessagesBeingSubscribed();
                            if (eventHandlers.Count != 0 || commandHandlers.Count != 0)
                            {
                                method.AddInvocationStatement($"services.Configure<{this.GetSubscriptionOptionsName()}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var commandHandler in commandHandlers)
                                        {
                                            if (!commandHandler.Element.IsIntegrationCommandModel()) continue;
                                            var commandName = this.GetIntegrationCommandName(commandHandler.Element.AsIntegrationCommandModel());
                                            arg.AddStatement($"""options.Add<{commandName}, {this.GetIntegrationEventHandlerInterfaceName()}<{commandName}>>();""");
                                        }

                                        foreach (var eventHandler in eventHandlers)
                                        {
                                            if (!eventHandler.Element.IsMessageModel()) continue;
                                            var messageName = this.GetIntegrationEventMessageName(eventHandler.Element.AsMessageModel());
                                            arg.AddStatement($"""options.Add<{messageName}, {this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>>();""");
                                        }
                                    }));
                            }

                            method.AddStatement("return services;");
                        });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureAzureServiceBus", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this)
                .ForConcern("Infrastructure"));

            // Process commands being sent
            foreach (var commandModel in GetCommandsBeingSent().DistinctBy(k => k.GetCommandQueueOrTopicConfigurationName()))
            {
                RegisterInfrastructureForModel(
                    commandModel.GetCommandQueueOrTopicConfigurationName(),
                    commandModel.GetCommandQueueOrTopicName(),
                    commandModel.GetCommandInfrastructureRegistrationEventName(),
                    commandModel.InternalElement.Package.ApplicationId
                );
            }

            // Process messages being published
            foreach (var messageModel in GetMessagesBeingPublished().DistinctBy(k => k.GetMessageQueueOrTopicConfigurationName()))
            {
                RegisterInfrastructureForModel(
                    messageModel.GetMessageQueueOrTopicConfigurationName(),
                    messageModel.GetMessageQueueOrTopicName(),
                    messageModel.GetMessageInfrastructureRegistrationEventName(),
                    messageModel.InternalElement.Package.ApplicationId
                );
            }

            // Process command subscriptions
            foreach (var commandModel in GetCommandMessagesBeingSubscribed()
                         .Select(x => x.Element.AsIntegrationCommandModel())
                         .Where(x => x?.HasCommandSubscription() == true)
                         .DistinctBy(k => k.GetCommandSubscriptionConfigurationName()))
            {
                RegisterSubscription(
                    commandModel.GetCommandSubscriptionConfigurationName()!,
                    commandModel.GetCommandQueueOrTopicName(),
                    commandModel.GetCommandQueueOrTopicConfigurationName(),
                    commandModel.GetCommandInfrastructureRegistrationEventName(),
                    commandModel.InternalElement.Package.ApplicationId
                );
            }

            // Process event subscriptions
            foreach (var messageModel in GetEventMessagesBeingSubscribed()
                         .Select(x => x.Element.AsMessageModel())
                         .Where(x => x?.HasMessageSubscription() == true)
                         .DistinctBy(k => k.GetMessageSubscriptionConfigurationName()))
            {
                RegisterSubscription(
                    messageModel.GetMessageSubscriptionConfigurationName()!,
                    messageModel.GetMessageQueueOrTopicName(),
                    messageModel.GetMessageQueueOrTopicConfigurationName(),
                    messageModel.GetMessageInfrastructureRegistrationEventName(),
                    messageModel.InternalElement.Package.ApplicationId
                );
            }
        }

        private void RegisterInfrastructureForModel(
            string configurationName,
            string queueOrTopicName,
            string registrationEventName,
            string modelApplicationId)
        {
            var fullConfigName = $"AzureServiceBus:{configurationName}";
            this.ApplyAppSetting(fullConfigName, queueOrTopicName);

            PublishInfrastructureRegisteredEvent(
                registrationEventName,
                queueOrTopicName,
                fullConfigName,
                modelApplicationId);
        }

        private void RegisterSubscription(
            string subscriptionConfigName,
            string queueOrTopicName,
            string queueOrTopicConfigName,
            string registrationEventName,
            string modelApplicationId)
        {
            var fullSubscriptionConfigName = $"AzureServiceBus:{subscriptionConfigName}";
            var fullQueueConfigName = $"AzureServiceBus:{queueOrTopicConfigName}";

            this.ApplyAppSetting(fullQueueConfigName, queueOrTopicName);
            this.ApplyAppSetting(fullSubscriptionConfigName, "");

            PublishInfrastructureRegisteredEvent(
                registrationEventName,
                queueOrTopicName,
                fullQueueConfigName,
                modelApplicationId);

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.AzureServiceBus.SubscriptionType)
                .WithProperty(Infrastructure.AzureServiceBus.Property.QueueOrTopicName, queueOrTopicName)
                .WithProperty(Infrastructure.AzureServiceBus.Property.ConfigurationName, fullSubscriptionConfigName));
        }

        private void PublishInfrastructureRegisteredEvent(
            string registrationEventName,
            string queueOrTopicName,
            string configurationName,
            string modelApplicationId)
        {
            var isExternal = modelApplicationId != ExecutionContext.GetApplicationConfig().Id;

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(registrationEventName)
                .WithProperty(Infrastructure.AzureServiceBus.Property.QueueOrTopicName, queueOrTopicName)
                .WithProperty(Infrastructure.AzureServiceBus.Property.ConfigurationName, configurationName)
                .WithProperty(Infrastructure.AzureServiceBus.Property.External, isExternal.ToString().ToLower()));
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

        private IReadOnlyList<SubscribeIntegrationCommandTargetEndModel> GetCommandMessagesBeingSubscribed()
        {
            return ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationCommandSubscriptions())
                .ToArray();
        }

        private IReadOnlyList<SubscribeIntegrationEventTargetEndModel> GetEventMessagesBeingSubscribed()
        {
            return ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationEventSubscriptions())
                .ToArray();
        }

        private IReadOnlyList<MessageModel> GetMessagesBeingPublished()
        {
            return ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(OutputTarget.Application)
                .ToArray();
        }

        private IReadOnlyList<IntegrationCommandModel> GetCommandsBeingSent()
        {
            return ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(OutputTarget.Application)
                .ToArray();
        }
    }
}