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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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
        public const string TemplateId = "Intent.Modules.Eventing.AzureServiceBus.AzureServiceBusConfiguration";

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
                            if (publishMessages.Length != 0 || sendCommands.Length != 0)
                            {
                                method.AddInvocationStatement($"services.Configure<{this.GetPublisherOptionsName()}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var sendCommand in sendCommands)
                                        {
                                            arg.AddStatement($"""options.Add<{this.GetIntegrationCommandName(sendCommand)}>(configuration["AzureServiceBus:{sendCommand.GetCommandQueueOrTopicConfigurationName()}"]!);""");
                                        }
                                        foreach (var publishMessage in publishMessages)
                                        {
                                            arg.AddStatement($"""options.Add<{this.GetIntegrationEventMessageName(publishMessage)}>(configuration["AzureServiceBus:{publishMessage.GetMessageQueueOrTopicConfigurationName()}"]!);""");
                                        }
                                    }));
                            }

                            var eventHandlers = GetEventMessagesBeingSubscribed();
                            var commandHandlers = GetCommandMessagesBeingSubscribed();
                            if (eventHandlers.Length != 0 || commandHandlers.Length != 0)
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
            foreach (var commandModel in GetCommandsBeingSent())
            {
                this.ApplyAppSetting($"AzureServiceBus:{commandModel.GetCommandQueueOrTopicConfigurationName()}", commandModel.GetCommandQueueOrTopicName());
            }

            foreach (var messageModel in GetMessagesBeingPublished())
            {
                this.ApplyAppSetting($"AzureServiceBus:{messageModel.GetMessageQueueOrTopicConfigurationName()}", messageModel.GetMessageQueueOrTopicName());
            }

            foreach (var commandModel in GetCommandMessagesBeingSubscribed()
                         .Select(x => x.Element.AsIntegrationCommandModel())
                         .Where(x => x?.HasCommandSubscription() == true))
            {
                this.ApplyAppSetting($"AzureServiceBus:{commandModel.GetCommandSubscriptionConfigurationName()}", "");
            }
            
            foreach (var messageModel in GetEventMessagesBeingSubscribed()
                         .Select(x => x.Element.AsMessageModel())
                         .Where(x => x?.HasMessageSubscription() == true))
            {
                this.ApplyAppSetting($"AzureServiceBus:{messageModel.GetMessageSubscriptionConfigurationName()}", "");
            }
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

        private SubscribeIntegrationCommandTargetEndModel[] GetCommandMessagesBeingSubscribed()
        {
            return ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationCommandSubscriptions())
                .ToArray();
        }

        private SubscribeIntegrationEventTargetEndModel[] GetEventMessagesBeingSubscribed()
        {
            return ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationEventSubscriptions())
                .ToArray();
        }

        private MessageModel[] GetMessagesBeingPublished()
        {
            return ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(OutputTarget.Application)
                .ToArray();
        }

        private IntegrationCommandModel[] GetCommandsBeingSent()
        {
            return ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(OutputTarget.Application)
                .ToArray();
        }
    }
}