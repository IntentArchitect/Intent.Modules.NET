using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineRegistrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Wolverine.WolverineRegistrationFactoryExtension";

        // Deliberate, not incidental: this must run after Intent.Wolverine.Common's
        // WolverineHostRegistrationExtension (Order 0), which establishes the shared
        // UseWolverine(opts => ...) lambda, and before Intent.Eventing.Wolverine (Order 20), whose
        // transport configuration is layered on top of this module's core configuration. Statements
        // land inside the lambda in ascending Order, so changing this value reorders the generated
        // Program.cs.
        [IntentManaged(Mode.Ignore)]
        public override int Order => 10;

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
            // Wolverine host registration is ASP.NET-host-only ("App.Program"). Azure Functions is
            // deliberately out of scope - it never worked correctly under any TypeLoadMode (see this
            // module's CONTEXT.md), so do not restore a host loop for it. A host-scoped template can
            // have more than one instance in a multi-host application (e.g. App.Api + Mobile.Api),
            // so this must use the plural lookup and loop rather than FindTemplateInstance, which
            // throws once a second host exists.
            foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
            {
                RegisterWolverineOnHost(programTemplate);
            }
        }

        private static void RegisterWolverineOnHost(IProgramTemplate programTemplate)
        {
            if (programTemplate == null)
            {
                return;
            }

            programTemplate.CSharpFile.OnBuild(file =>
            {
                // Appends to the shared lambda that Intent.Wolverine.Common already established at
                // Order -10. ConfigureHostBuilderChainStatement is find-or-create, so this resolves
                // that same lambda rather than emitting a competing registration.
                //
                // Intent.Wolverine.Common owns the WolverineFx package reference and the
                // "using Wolverine" on this file - do not re-declare either here.
                //
                // NEVER call lambdaBlock.Statements.Clear(). Doing so discards whatever another
                // contributor has already added, and was the original defect that made this whole
                // area order-dependent.
                programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" },
                    (lambdaBlock, parameters) =>
                    {
                        var opts = parameters[0];
                        var wolverineConfigType = programTemplate.GetTypeName("Intent.Application.Wolverine.WolverineConfiguration");
                        lambdaBlock.AddStatement($"{wolverineConfigType}.Configure({opts});");
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
