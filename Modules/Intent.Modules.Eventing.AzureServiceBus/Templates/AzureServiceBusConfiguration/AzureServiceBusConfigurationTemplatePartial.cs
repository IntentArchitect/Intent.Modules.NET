using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
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
                                .AddParameter("IConfiguration", "configuration")

                                .AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureServiceBusEventBusName()}>();")
                                .AddStatement($"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherName()}>();")
                                .AddStatement(
                                    $"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherInterfaceName()}, {this.GetAzureServiceBusMessageDispatcherName()}>();");
                            // .AddStatement("services.Configure<PublisherOptions>(options =>")
                            // .AddBlock(block =>
                            // {
                            //     block.AddStatement("options.Add<CreateOrgIntegrationCommand>(configuration[\"AzureServiceBus:CreateOrg\"]!);")
                            //          .AddStatement("options.Add<ClientCreatedEvent>(configuration[\"AzureServiceBus:ClientCreated\"]!);")
                            //          .AddStatement("options.Add<SpecificQueueOneMessageEvent>(configuration[\"AzureServiceBus:SpecificQueue\"]!);")
                            //          .AddStatement("options.Add<SpecificQueueTwoMessageEvent>(configuration[\"AzureServiceBus:SpecificQueue\"]!);")
                            //          .AddStatement("options.Add<SpecificTopicOneMessageEvent>(configuration[\"AzureServiceBus:SpecificTopic\"]!);")
                            //          .AddStatement("options.Add<SpecificTopicTwoMessageEvent>(configuration[\"AzureServiceBus:SpecificTopic\"]!);");
                            // })
                            // .AddStatement("services.Configure<SubscriptionOptions>(options =>")
                            // .AddBlock(block =>
                            // {
                            //     block.AddStatement("options.Add<CreateOrgIntegrationCommand, IIntegrationEventHandler<CreateOrgIntegrationCommand>>();")
                            //          .AddStatement("options.Add<ClientCreatedEvent, IIntegrationEventHandler<ClientCreatedEvent>>();")
                            //          .AddStatement("options.Add<SpecificQueueOneMessageEvent, IIntegrationEventHandler<SpecificQueueOneMessageEvent>>();")
                            //          .AddStatement("options.Add<SpecificQueueTwoMessageEvent, IIntegrationEventHandler<SpecificQueueTwoMessageEvent>>();")
                            //          .AddStatement("options.Add<SpecificTopicOneMessageEvent, IIntegrationEventHandler<SpecificTopicOneMessageEvent>>();")
                            //          .AddStatement("options.Add<SpecificTopicTwoMessageEvent, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>>();");
                            // })
                            method.AddStatement("return services;");
                        });
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