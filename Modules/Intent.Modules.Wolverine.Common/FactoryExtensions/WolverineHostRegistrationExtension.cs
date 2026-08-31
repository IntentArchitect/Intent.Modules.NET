using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
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

        // Runs ahead of every Wolverine-based contributor (Intent.Application.Wolverine at 10,
        // Intent.Eventing.Wolverine at 20) so that this module, not whichever contributor happened
        // to execute first, is what establishes the shared UseWolverine(opts => ...) lambda. See
        // SeedWolverineHostRegistration for why that ordering is the whole mechanism.
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
        /// Establishes the single <c>builder.Host.UseWolverine(opts => ...)</c> registration on the
        /// given ASP.NET host, and owns the <c>WolverineFx</c> package and <c>using Wolverine</c>
        /// that registration needs. Contributing modules do not re-declare either.
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

                // Seed the lambda with no statements of its own. ConfigureHostBuilderChainStatement
                // is find-or-create: it looks for an existing "builder.Host.UseWolverine(" statement
                // and only creates one when absent, so every later caller naming "UseWolverine"
                // resolves to THIS lambda instead of emitting a competing registration.
                //
                // Seeding it here, from a factory extension ordered ahead of every contributor, is
                // what makes the result deterministic in two ways that matter:
                //   1. the position of the UseWolverine statement within Program.cs no longer
                //      depends on which contributing modules happen to be installed, and
                //   2. contributions land in ascending factory-extension Order, because each
                //      contributor's OnBuild callback is registered on this same CSharpFile during
                //      its own OnAfterTemplateRegistrations, and OnBuild callbacks fire in
                //      registration order.
                //
                // Ordering cannot be expressed through the DSL's own `priority` parameter: the
                // ASP.NET implementation of ConfigureHostBuilderChainStatement accepts it and never
                // reads it, and the ConfigureServices callback it delegates to has no priority
                // parameter at all. Factory-extension Order is the only lever.
                programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" });
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
