using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    public class FrameworkDependencyResolutionFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.VSProjects.FrameworkDependencyResolver";

        public override int Order => 0;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                ResolveFrameworkDependencies(application);
            }
        }

        public void ResolveFrameworkDependencies(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info("Resolving Framework Dependencies");

            foreach (var outputTarget in application.OutputTargets)
            {
                var project = outputTarget.GetTargetPath()[0]; // root is the project itself

                foreach (var frameworkDependency in GetAllTemplateFrameworkDependencies(outputTarget))
                {
                    project.AddFrameworkDependency(frameworkDependency);
                }
            }
        }

        internal static IEnumerable<string> GetAllTemplateFrameworkDependencies(IOutputTarget outputTarget)
        {
            return outputTarget.TemplateInstances
                .ExceptHandsOff()
                .SelectMany(ti => ti.GetAllFrameworkDependencies());
        }
    }
}

