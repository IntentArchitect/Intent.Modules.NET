using Intent.Engine;
using Intent.Modules.Blazor.Templates.Templates.AI.BlazorDialogAddingEntitySkill;
using Intent.Modules.Blazor.Templates.Templates.StaticContentTemplateRegistrations;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SkillSampleFileOutputFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.SkillSampleFileOutputFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //var skillTemplate = application.FindTemplateInstances<MarkdownBaseTemplate<object>>(BlazorDialogAddingEntitySkillTemplate.TemplateId);

            //var dialogAddSkill = application.FindTemplateInstance<MarkdownBaseTemplate<object>>(BlazorDialogAddingEntitySkillTemplate.TemplateId);
            //var dialogAddSkillSample = application.FindTemplateInstance<StaticContentTemplateRegistration>(BlazorDialogAddingEntitySkillSampleFilesStaticContentTemplateRegistration.TemplateId);
        }

    }
}