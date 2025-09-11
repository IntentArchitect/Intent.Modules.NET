using Intent.Engine;
using Intent.Modules.Application.DomainInteractions.InteractionStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Contracts.InteractionStrategies;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.DomainInteractions.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainInteractionRegistration : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.DomainInteractions.DomainInteractionRegistration";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            InteractionStrategyProvider.Instance.Register(new ODataQueryInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new QueryInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new CallEntityServiceInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new CreateEntityInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new UpdateEntityInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new DeleteEntityInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new ProcessingActionInteractionStrategy());
            InteractionStrategyProvider.Instance.Register(new CallDomainServiceInteractionStrategy());
        }
    }
}