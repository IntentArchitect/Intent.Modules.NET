using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.UnitTesting.Settings;
using Intent.Modules.UnitTesting.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.UnitTesting.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NuGetInstallFactoryExtension : FactoryExtensionBase
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string UnitTestRole = "UnitTests";

        public override string Id => "Intent.UnitTesting.NuGetInstallFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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
            var target = application.OutputTargets.FirstOrDefault(x => x.HasRole(UnitTestRole));

            if (target != null)
            {
                var project = target.GetProject();
                var mockNuget = TestHelpers.GetMockFramework(application.Settings.GetUnitTestSettings().MockFramework().AsEnum(), target);

                List<INugetPackageInfo> packages = [NugetPackages.XunitV3(target), NugetPackages.CoverletCollector(target),
                   NugetPackages.XunitRunnerVisualstudio(target), NugetPackages.MicrosoftNETTestSdk(target), mockNuget];
                project.AddNugetPackages(packages);
            }
        }
    }
}