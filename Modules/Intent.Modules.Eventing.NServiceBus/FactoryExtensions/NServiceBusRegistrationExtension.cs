using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageBus;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using NServiceBusConstants = Intent.Modules.Eventing.NServiceBus.Templates.Constants;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NServiceBusRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.NServiceBus.NServiceBusRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 500;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            MessageBusRegistry.Register(NServiceBusConstants.NServiceBusMessageBusId, NServiceBusConstants.BrokerStereotypeIds);
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // DI registration is owned by NServiceBusConfigurationTemplate.AddNServiceBusConfiguration —
            // in single-broker mode it publishes a ServiceConfigurationRequest that wires Infrastructure DI,
            // in multi-broker mode the composite discovers the same method via the MessageBusConfiguration role.
            // This factory extension only handles app-settings + Program.cs host-builder wiring.

            var transport = application.Settings.GetNServiceBusSettings().Transport();
            var recoverabilityPolicy = application.Settings.GetNServiceBusSettings().RecoverabilityPolicy();

            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("NServiceBus", new { EndpointName = "MyApplication" }));

            if (!recoverabilityPolicy.IsNone())
            {
                if (recoverabilityPolicy.IsImmediateOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("NServiceBus:Recoverability", new { ImmediateRetries = 5 }));
                if (recoverabilityPolicy.IsDelayedOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("NServiceBus:Recoverability", new { DelayedRetries = 3, DelayIncreaseSeconds = 10 }));
                application.EventDispatcher.Publish(new AppSettingRegistrationRequest("NServiceBus", new { ErrorQueue = "error" }));
            }

            if (transport.IsRabbitmq())
                application.EventDispatcher.Publish(new AppSettingRegistrationRequest("ConnectionStrings", new { RabbitMQ = "host=localhost" }));
            else if (transport.IsAzureServiceBus())
                application.EventDispatcher.Publish(new AppSettingRegistrationRequest("ConnectionStrings",
                    new { AzureServiceBus = "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key" }));
            else if (transport.IsSqlServer())
                application.EventDispatcher.Publish(new AppSettingRegistrationRequest("ConnectionStrings",
                    new { SqlServer = "Server=localhost;Database=NServiceBus;Integrated Security=True;" }));
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var messageBusTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                NServiceBusMessageBusTemplate.TemplateId);

            if (messageBusTemplate != null)
            {
                var diTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                    TemplateRoles.Infrastructure.DependencyInjection);
                if (diTemplate != null)
                {
                    var busInterfaceTemplate = application.FindTemplateInstance<IClassProvider>(
                        TemplateRoles.Application.Eventing.MessageBusInterface);
                    if (busInterfaceTemplate != null)
                    {
                        diTemplate.CSharpFile.OnBuild(file =>
                        {
                            file.AddUsing(busInterfaceTemplate.Namespace);
                        }, 500);
                    }
                }
            }

            var programTemplate = application.FindTemplateInstance<IProgramTemplate>("App.Program");
            if (programTemplate == null) return;

            var configTemplate = application.FindTemplateInstance<IClassProvider>(
                NServiceBusConfigurationTemplate.TemplateId);
            if (configTemplate == null) return;

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(configTemplate.Namespace);
                programTemplate.ProgramFile.AddHostBuilderConfigurationStatement(
                    new CSharpStatement("builder.Host.AddNServiceBus(builder.Configuration);"));
            }, 10);
        }
    }
}
