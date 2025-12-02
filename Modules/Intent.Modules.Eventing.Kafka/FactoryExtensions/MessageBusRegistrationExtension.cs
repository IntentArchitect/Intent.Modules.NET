using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Contracts;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Kafka.MessageBusRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            const string KafkaMessageBusId = "c0f7166a-018a-453a-b9c0-48f690ec55fb";
            MessageBusRegistry.Register(KafkaMessageBusId, Templates.Constants.BrokerStereotypeIds);
        }
    }
}