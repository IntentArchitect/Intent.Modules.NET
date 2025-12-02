using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Contracts;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.AzureEventGrid.MessageBusRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            const string AzureEventGridMessageBusId = "48880079-8788-4c53-b7f3-0cbc7c4c8a88";
            MessageBusRegistry.Register(AzureEventGridMessageBusId, Templates.Constants.BrokerStereotypeIds);
        }
    }
}