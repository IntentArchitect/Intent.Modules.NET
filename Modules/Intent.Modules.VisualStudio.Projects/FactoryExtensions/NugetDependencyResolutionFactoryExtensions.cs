using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    [Description("Visual Studio Dependancy Resolver")]
    public class NugetDependencyResolutionFactoryExtensions : FactoryExtensionBase
    {
        public override string Id => "Intent.VSProjects.NuGetDependencyResolver";

        public override int Order => 1000;

        public override void OnStep(IApplication application, string step)
        {
            switch (step)
            {
                case ExecutionLifeCycleSteps.AfterTemplateRegistrations:
                {
                    foreach (var project in application.OutputTargets.Where(x => x.IsVSProject()))
                    {
                        project.InitializeVSMetadata();
                    }

                    break;
                }
                case ExecutionLifeCycleSteps.BeforeTemplateExecution:
                {
                    ResolveNuGetDependencies(application);
                    break;
                }
            }
        }

        public void ResolveNuGetDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Resolving NuGet Dependencies");

            foreach (var outputTarget in application.OutputTargets)
            {
                var project = outputTarget.GetProject();

                project.AddNugetPackageInstalls(GetAllTemplateNuGetInstalls(outputTarget));

                var assemblyDependencies = outputTarget.TemplateInstances
                        .SelectMany(ti => ti.GetAllAssemblyDependencies())
                        .Distinct()
                        .ToList();

                foreach (var assemblyDependency in assemblyDependencies)
                {
                    project.AddReference(assemblyDependency);
                }
            }
        }

        private static IEnumerable<NuGetInstall> GetAllTemplateNuGetInstalls(IOutputTarget outputTarget)
        {
            return outputTarget.TemplateInstances
                    .SelectMany(ti => ti.GetAllNuGetInstalls())
                    .Distinct()
                    .ToList();
        }
    }
}

