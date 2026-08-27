using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Wolverine.Common.Api;
using Intent.Modules.Wolverine.Common.FactoryExtensions;
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
            // Wolverine host registration is ASP.NET-host-only ("App.Program"). Azure Functions is
            // deliberately out of scope (see Intent.Wolverine.Common's own registration extension,
            // which owns the shared UseWolverine(opts => ...) lambda). A host-scoped template can
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

            programTemplate.AddNugetDependency(NugetPackages.WolverineFx(programTemplate.OutputTarget));

            programTemplate.CSharpFile.OnBuild(file => file.AddUsing("Wolverine"));

            // Contribute() must be called NOW, synchronously during OnAfterTemplateRegistrations -
            // not deferred inside a CSharpFile.OnBuild callback. WolverineHostRegistrationExtension
            // (Intent.Wolverine.Common) reads the contributions table from its OWN OnBuild callback
            // on this same CSharpFile; OnBuild callbacks fire in registration order, so a contribution
            // registered inside OnBuild here could lose the race against Common's consuming callback
            // depending on factory-extension execution order, silently vanishing from the generated
            // lambda. Calling Contribute() eagerly here guarantees the entry exists in the table
            // before ANY OnBuild callback runs. The type name resolution still happens lazily inside
            // the ConfigureAction closure, which Common invokes during its own OnBuild callback
            // (after every template has been registered).
            WolverineHostRegistrationExtension.Contribute(programTemplate,
                WolverineHostConfigurationRequest.Configure((lambdaBlock, parameters) =>
                {
                    var opts = parameters[0];
                    var wolverineConfigType = programTemplate.GetTypeName("Intent.Application.Wolverine.WolverineConfiguration");
                    lambdaBlock.AddStatement($"{wolverineConfigType}.Configure({opts});");
                }));
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