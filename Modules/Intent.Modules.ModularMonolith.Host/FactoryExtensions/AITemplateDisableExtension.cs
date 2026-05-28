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
            // on the host, don't generate any AI skills/instructions, these shold come from the modules
            var aiSkillTemplates = application.FindTemplateInstances<IIntentTemplate>("AI.Context.Skills");
            foreach (var item in aiSkillTemplates)
            {
                item.CanRun = false;
            }

            var aiInstructionTemplates = application.FindTemplateInstances<IIntentTemplate>("AI.Context.Instructions");
            foreach (var item in aiInstructionTemplates)
            {
                item.CanRun = false;
            }
        }
    }
}