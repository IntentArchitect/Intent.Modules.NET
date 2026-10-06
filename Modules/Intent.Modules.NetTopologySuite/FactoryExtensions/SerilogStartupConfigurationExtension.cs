using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.NetTopologySuite.Templates.GeoDestructureSerilogPolicy;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SerilogStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.NetTopologySuite.SerilogStartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

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
            foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>(TemplateRoles.Distribution.WebApi.Program))
            {
                programTemplate.CSharpFile.OnBuild(file =>
                {
                    programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseSerilog", ["context", "services", "configuration"],
                        (lambdaBlock, parameters) =>
                        {
                            // Resolve the reachable instance and name it directly: the template-id GetTypeName
                            // overload falls back to an unscoped lookup that picks up (or trips over) other hosts' policies.
                            if (programTemplate.OutputTarget.FindTemplateInstance(GeoDestructureSerilogPolicyTemplate.TemplateId) is not { } policyTemplate ||
                                !policyTemplate.CanRunTemplate())
                            {
                                Logging.Log.Warning(
                                    $"NetTopologySuite: project '{programTemplate.OutputTarget.GetProject().Name}' configures Serilog but has no geometry destructuring policy it can reference, " +
                                    "so logging a NetTopologySuite geometry there can recurse through its circular references. " +
                                    $"To include it, add a Template Output for {GeoDestructureSerilogPolicyTemplate.TemplateId} under that project in the Codebase Structure designer.");
                                return;
                            }

                            var chain = (CSharpMethodChainStatement)lambdaBlock.Statements.First();
                            chain.AddChainStatement($"Destructure.With(new {programTemplate.GetTypeName(policyTemplate)}())");
                        });
                }, 15);
            }
        }
    }
}
