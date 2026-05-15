using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
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

        // TODO: Add NServiceBus endpoint configuration and IHostedService wiring here.
        // This requires generating NServiceBusConfiguration.cs (static extension method) and
        // NServiceBusHostedService.cs, then finding the infrastructure DI template and calling them.
        // The IMessageBus -> NServiceBusMessageBus registration is handled by NServiceBusPublisherTemplate.BeforeTemplateExecution.
    }
}