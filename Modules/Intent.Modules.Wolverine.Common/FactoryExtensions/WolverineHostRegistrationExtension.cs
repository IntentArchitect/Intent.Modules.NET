using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Wolverine.Common.Templates.WolverineConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Wolverine.Common.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineHostRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Wolverine.Common.WolverineHostRegistrationExtension";

        // This module is the only one that ever reaches into Program.cs for Wolverine - contributing
        // modules (Intent.Application.Wolverine, Intent.Eventing.Wolverine) target this module's own
        // WolverineConfiguration template instead, at Order 10/20 there. This Order value only decides
        // where the WolverineConfiguration.Configure(...) statement LANDS in Program.cs itself.
        //
        // Deliberately left at 0 rather than moved negative. Where the generated
        // builder.Host.UseWolverine(...) statement LANDS in Program.cs depends on when the DSL's
        // ConfigureServices callback is queued relative to the ones adding builder.Services.*.
        // Seeding from a negative Order queues it earlier and relocates the statement below the
        // builder.Services block - taking the neighbouring builder.Host.UseSerilog(...) call, owned
        // by another module, along with it. 0 is the value the previous implementation effectively
        // used, so it keeps placement unchanged; the contributors move out instead.
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
            // This module only ever targets the ASP.NET host role. Azure Functions is deliberately
            // excluded - it has its own program template shape and is out of scope for this module.
            // A host-scoped template can have more than one instance in a multi-host application, so
            // this uses the plural lookup rather than FindTemplateInstance, which throws once a
            // second host exists.
            foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
            {
                SeedWolverineHostRegistration(programTemplate);
            }
        }

        /// <summary>
        /// Establishes the single <c>builder.Host.UseWolverine(opts => WolverineConfiguration.Configure(opts,
        /// builder.Configuration))</c> registration on the given ASP.NET host, and owns the <c>WolverineFx</c>
        /// package and <c>using Wolverine</c> that registration needs.
        /// <para>
        /// This is the ONLY place any Wolverine module touches <c>Program.cs</c>. Contributing modules
        /// (<c>Intent.Application.Wolverine</c>, <c>Intent.Eventing.Wolverine</c>) no longer reach into the
        /// host builder at all - they instead find <see cref="WolverineConfigurationTemplate"/> and add
        /// their own private method plus one call statement to its <c>Configure</c> method body. See
        /// their own FactoryExtensions for that contribution mechanism.
        /// </para>
        /// </summary>
        private static void SeedWolverineHostRegistration(IProgramTemplate programTemplate)
        {
            if (programTemplate == null)
            {
                return;
            }

            // Outside the OnBuild callback deliberately - NuGet dependencies declared inside a build
            // callback are not reliably picked up.
            programTemplate.AddNugetDependency(NugetPackages.WolverineFx(programTemplate.OutputTarget));

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Wolverine");

                // ConfigureHostBuilderChainStatement is find-or-create: it looks for an existing
                // "builder.Host.UseWolverine(" statement and only creates one when absent. Since this
                // module is now the only contributor to Program.cs, this both creates the lambda AND
                // supplies its one statement in the same call - there is nothing left for another
                // module to append.
                var configType = programTemplate.GetTypeName(WolverineConfigurationTemplate.TemplateId);
                programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" },
                    (lambdaBlock, parameters) =>
                    {
                        var opts = parameters[0];
                        lambdaBlock.AddStatement($"{configType}.Configure({opts}, builder.Configuration);");
                    });
            });
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
        // Your custom logic here.
        }
    }
}
