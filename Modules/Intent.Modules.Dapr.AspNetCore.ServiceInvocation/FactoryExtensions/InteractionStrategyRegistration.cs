using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Integration.HttpClients.Shared.InteractionStrategies;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class InteractionStrategyRegistration : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.ServiceInvocation.InteractionStrategyRegistration";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            InteractionStrategyProvider.Instance.Register(new CqrsInvocationHttpInteractionStrategy(application));
        }
    }
}