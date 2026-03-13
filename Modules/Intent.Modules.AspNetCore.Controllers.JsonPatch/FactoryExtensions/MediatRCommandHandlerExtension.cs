using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MediatRCommandHandlerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonPatch.MediatRCommandHandlerExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            // Override the default UpdateEntityInteractionStrategy for PATCH commands.
            InteractionStrategyProvider.Instance.Register(new PatchUpdateEntityInteractionStrategy(), priority: -100);
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
        }
    }
}