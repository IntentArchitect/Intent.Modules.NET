using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SonarQube.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AddAnalyzerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SonarQube.AddAnalyzerExtension";

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
           
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            foreach (var target in application.OutputTargets)
            {
                if (target.IsVSProject())
                {
                    target.GetProject().AddNugetPackages([NugetPackages.SonarAnalyzerCSharp(target)]);
                }
            }
        }
    }
}