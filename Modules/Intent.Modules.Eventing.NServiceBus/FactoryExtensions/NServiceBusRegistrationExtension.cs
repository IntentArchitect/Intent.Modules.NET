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
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var messageBusTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateRoles.Application.Eventing.MessageBusImplementation);
            if (messageBusTemplate == null) return;

            application.EventDispatcher.Publish(
                ContainerRegistrationRequest.ToRegister(messageBusTemplate)
                    .ForInterface(messageBusTemplate.GetBusInterfaceName())
                    .ForConcern("Infrastructure")
                    .WithPriority(100)
                    .WithPerServiceCallLifeTime());

            var transport = application.Settings.GetNServiceBusSettings().Transport();

            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("NServiceBus", new { EndpointName = "MyApplication" }));

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
            var programTemplate = application.FindTemplateInstance<IProgramTemplate>("App.Program");
            if (programTemplate == null) return;

            var configTemplate = application.FindTemplateInstance<IClassProvider>(
                NServiceBusConfigurationTemplate.TemplateId);
            if (configTemplate == null) return;

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(configTemplate.Namespace);
                programTemplate.ProgramFile.AddHostBuilderConfigurationStatement(
                    new CSharpStatement("builder.Host.AddNServiceBus(builder.Configuration)"));
            }, 10);
        }
    }
}
