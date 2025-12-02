using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Eventing.Contracts.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Contracts;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageBusRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.Pubsub.MessageBusRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            const string DaprMessageBusId = "764c5213-7e84-4b10-9414-addde0c07b69";
            MessageBusRegistry.Register(DaprMessageBusId, Templates.Constants.BrokerStereotypeIds);
        }
    }
}