using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class MassTransitConfigurationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MassTransitConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MassTransit);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(
                    "AddMassTransitConfiguration",
                    ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
            {
                case Settings.EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                    // InMemory doesn't require appsettings
                    break;
                case Settings.EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                    AddNugetDependency(NuGetPackages.MassTransitRabbitMq);

                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Host", "localhost"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:VirtualHost", "/"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Username", "guest"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("RabbitMq:Password", "guest"));
                    break;
                case Settings.EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                    AddNugetDependency(NuGetPackages.MassTransitAzureServiceBusCore);

                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AzureMessageBus:ConnectionString", "your connection string"));
                    break;
                case Settings.EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                    AddNugetDependency(NuGetPackages.MassTransitAmazonSqs);

                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AmazonSqs:Host", "us-east-1"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AmazonSqs:AccessKey", "your-iam-access-key"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AmazonSqs:SecretKey", "your-iam-secret-key"));
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"MassTransitConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private ScopedExtensionMethodConfiguration _messageProviderSpecificConfigCode;

        public ScopedExtensionMethodConfiguration MessageProviderSpecificConfigCode
        {
            get { return _messageProviderSpecificConfigCode ??= GetGetMessagingProviderSpecificCode(); }
        }

        public List<ScopedExtensionMethodConfiguration> AdditionalConfiguration { get; } = new();

        private string GetConsumers()
        {
            var consumers = new List<string>();
            foreach (var messageHandlerModel in ExecutionContext.MetadataManager
                         .Eventing(ExecutionContext.GetApplicationConfig().Id).GetConsumerModels().SelectMany(x => x.MessageConsumers))
            {
                var messageName = this.GetIntegrationEventMessageName(messageHandlerModel.TypeReference.Element.AsMessageModel());
                consumers.Add($@"cfg.AddConsumer<{this.GetWrapperConsumerName()}<{this.GetIntegrationEventHandlerInterfaceName()}<{messageName}>, {messageName}>>();");
            }

            const string newLine = @"
                ";
            return string.Join(newLine, consumers);
        }

        private ScopedExtensionMethodConfiguration GetGetMessagingProviderSpecificCode()
        {
            switch (ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum())
            {
                case EventingSettings.MessagingServiceProviderOptionsEnum.InMemory:
                    return new ScopedExtensionMethodConfiguration("UsingInMemory", "context", "cfg").AppendNestedLines(new[]
                    {
                        $@"cfg.ConfigureEndpoints(context);"
                    });
                case EventingSettings.MessagingServiceProviderOptionsEnum.Rabbitmq:
                    return new ScopedExtensionMethodConfiguration("UsingRabbitMq", "context", "cfg").AppendNestedLines(new[]
                    {
                        $@"cfg.Host(configuration[""RabbitMq:Host""], configuration[""RabbitMq:VirtualHost""], h =>",
                        $@"{{",
                        $@"    h.Username(configuration[""RabbitMq:Username""]);",
                        $@"    h.Password(configuration[""RabbitMq:Password""]);",
                        $@"}});",
                        $@"",
                        $@"cfg.ConfigureEndpoints(context);"
                    });
                case EventingSettings.MessagingServiceProviderOptionsEnum.AzureServiceBus:
                    return new ScopedExtensionMethodConfiguration("UsingAzureServiceBus", "context", "cfg").AppendNestedLines(new[]
                    {
                        $@"cfg.Host(configuration[""AzureMessageBus:ConnectionString""]);",
                        $@"",
                        $@"cfg.ConfigureEndpoints(context);"
                    });
                case EventingSettings.MessagingServiceProviderOptionsEnum.AmazonSqs:
                    return new ScopedExtensionMethodConfiguration("UsingAmazonSqs", "context", "cfg").AppendNestedLines(new[]
                    {
                        $@"cfg.Host(configuration[""AmazonSqs:Host""], h =>",
                        $@"{{",
                        $@"    h.AccessKey(configuration[""AmazonSqs:AccessKey""]);",
                        $@"    h.SecretKey(configuration[""AmazonSqs:SecretKey""]);",
                        $@"}});",
                        $@"",
                        $@"cfg.ConfigureEndpoints(context);"
                    });
                default:
                    throw new InvalidOperationException(
                        $"Messaging Service Provider is set to a setting that is not supported: {ExecutionContext.Settings.GetEventingSettings().MessagingServiceProvider().AsEnum()}");
            }
        }

        private string GetMessagingProviderSpecificConfig()
        {
            var lines = new List<string>();

            lines.Add($@"x.{MessageProviderSpecificConfigCode.ExtensionMethodName}(({string.Join(", ", MessageProviderSpecificConfigCode.Parameters)}) =>");
            lines.Add(@$"{{");
            lines.AddRange(MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Select(s => $@"    {s}"));
            lines.Add(@$"}});");

            const string newLine = @"
                ";
            return newLine + string.Join(newLine, lines);
        }

        private string GetAdditionalConfiguration()
        {
            var lines = new List<string>();

            foreach (var extensionMethodConfiguration in AdditionalConfiguration)
            {
                lines.Add(@$"");
                lines.Add(@$"x.{extensionMethodConfiguration.ExtensionMethodName}(({string.Join(", ", extensionMethodConfiguration.Parameters)}) =>");
                lines.Add(@$"{{");
                lines.AddRange(extensionMethodConfiguration.NestedConfigurationCodeLines.Select(s => $@"    {s}"));
                lines.Add(@$"}});");
            }

            const string newLine = @"
                ";
            return newLine + string.Join(newLine, lines);
        }

        public class ScopedExtensionMethodConfiguration
        {
            public ScopedExtensionMethodConfiguration(string extensionMethodName, params string[] parameters)
            {
                ExtensionMethodName = extensionMethodName;
                Parameters = parameters ?? Array.Empty<string>();
            }

            public string ExtensionMethodName { get; }
            public IReadOnlyCollection<string> Parameters { get; }
            public List<string> NestedConfigurationCodeLines { get; } = new();

            public ScopedExtensionMethodConfiguration AppendNestedLines(IEnumerable<string> lines)
            {
                NestedConfigurationCodeLines.AddRange(lines);
                return this;
            }
        }
    }
}