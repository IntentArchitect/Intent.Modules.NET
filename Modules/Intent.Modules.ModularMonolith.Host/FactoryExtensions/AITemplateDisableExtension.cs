using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AITemplateDisableExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.AITemplateDisableExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // on the host, don't generate any AI skills/instructions, these should come from the modules.
            // Includes the narrower "AI.Context.Skills.Handler" role: handler skills were re-scoped to it
            // and would otherwise no longer be caught here, leaking onto the host and clobbering the
            // module-generated skill files (shared output folder).
            var rolesToDisable = new[]
            {
                "AI.Context.Skills",
                "AI.Context.Skills.Handler",
                "AI.Context.Instructions",
            };

            foreach (var role in rolesToDisable)
            {
                foreach (var item in application.FindTemplateInstances<IIntentTemplate>(role))
                {
                    item.CanRun = false;
                }
            }
        }
    }
}