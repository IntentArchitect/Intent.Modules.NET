using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.NetTopologySuite.Templates;
using Intent.Modules.NetTopologySuite.Templates.GeoDestructureSerilogPolicy;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
                            if (programTemplate.OutputTarget.FindTemplateInstance(GeoDestructureSerilogPolicyTemplate.TemplateId) is null)
                            {
                                return;
                            }

                            var chain = (CSharpMethodChainStatement)lambdaBlock.Statements.First();
                            chain.AddChainStatement($"Destructure.With(new {programTemplate.GetGeoDestructureSerilogPolicyName()}())");
                        });
                }, 15);
            }
        }
    }
}
