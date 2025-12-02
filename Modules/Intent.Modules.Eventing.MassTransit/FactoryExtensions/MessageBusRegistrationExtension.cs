using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Contracts;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.MessageBusRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            const string MassTransitMessageBusId = "c6185bdb-d69b-4db6-af0f-ef2fe07a783c";
            MessageBusRegistry.Register(MassTransitMessageBusId, Templates.Constants.BrokerStereotypeIds);
        }
    }
}