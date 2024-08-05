using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Ardalis.GuardClauses.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GuardClausesFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Ardalis.GuardClauses.GuardClausesFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var entityTemplates = application.FindTemplateInstances<ICSharpTemplate>(TemplateDependency.OnTemplate("Domain.Entity"));
            foreach (var template in entityTemplates)
            {
                template.AddNugetDependency(NugetPackages.ArdalisGuardClauses(template.OutputTarget));
                template.AddUsing("Ardalis.GuardClauses");
            }
        }
    }
}