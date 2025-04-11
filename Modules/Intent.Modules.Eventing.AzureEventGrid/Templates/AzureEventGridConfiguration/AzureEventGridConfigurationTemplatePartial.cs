using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureEventGridConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"AzureEventGridConfiguration", @class =>
                {
                    @class.Static()
                        .AddMethod("IServiceCollection", "ConfigureEventGrid", method =>
                        {
                            method.Static()
                                .AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                                .AddParameter("IConfiguration", "configuration");

                            method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureEventGridEventBusName()}>();")
                                .AddStatement($"services.AddSingleton<{this.GetAzureEventGridMessageDispatcherName()}>();")
                                .AddStatement(
                                    $"services.AddSingleton<{this.GetAzureEventGridMessageDispatcherInterfaceName()}, {this.GetAzureEventGridMessageDispatcherName()}>();");

                            var publishMessages = GetMessagesBeingPublished();
                            if (publishMessages.Count != 0)
                            {
                                method.AddInvocationStatement($"services.Configure<{this.GetPublisherOptionsName()}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var publishMessage in publishMessages)
                                        {
                                            arg.AddStatement(
                                                $"""options.Add<{this.GetIntegrationEventMessageName(publishMessage)}>(configuration["EventGrid:Topics:{publishMessage.GetTopicConfigurationName()}:Key"]!, configuration["EventGrid:Topics:{publishMessage.GetTopicConfigurationName()}:Endpoint"]!);""");
                                        }
                                    }));
                            }

                            var eventHandlers = GetEventMessagesBeingSubscribed();
                            if (eventHandlers.Count != 0)
                            {
                                method.AddInvocationStatement($"services.Configure<{this.GetSubscriptionOptionsName()}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
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
                .ToRegister("ConfigureEventGrid", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this)
                .ForConcern("Infrastructure"));

            foreach (var messageModel in GetMessagesBeingPublished().DistinctBy(k => k.GetTopicName()))
            {
                RegisterEventGridTopic(
                    messageModel.GetTopicConfigurationName(),
                    messageModel.InternalElement.Package.ApplicationId
                );
            }
        }

        private void RegisterEventGridTopic(string topicName, string modelApplicationId)
        {
            var keyConfigName = $"EventGrid:Topics:{topicName}:Key";
            var endpointConfigName = $"EventGrid:Topics:{topicName}:Endpoint";

            this.ApplyAppSetting(keyConfigName, "");
            this.ApplyAppSetting(endpointConfigName, "");

            var isExternal = modelApplicationId != ExecutionContext.GetApplicationConfig().Id;

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent("EventGridTopicRegistered")
                .WithProperty("TopicName", topicName)
                .WithProperty("KeyConfig", keyConfigName)
                .WithProperty("EndpointConfig", endpointConfigName)
                .WithProperty("External", isExternal.ToString().ToLower()));
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