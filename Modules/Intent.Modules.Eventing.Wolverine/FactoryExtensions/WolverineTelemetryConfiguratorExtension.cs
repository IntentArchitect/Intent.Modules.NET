using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineTelemetryConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Wolverine.WolverineTelemetryConfiguratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// R11.1: emits `AddSource("Wolverine")` into Intent.OpenTelemetry's tracing configuration,
        /// but only when that module is installed - a subscribe-only or OpenTelemetry-less
        /// application gets no reference to it at all.
        /// </summary>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var openTelemetryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration"));
            if (openTelemetryTemplate == null)
            {
                return;
            }

            openTelemetryTemplate.CSharpFile.OnBuild(AddWolverineTraceSource);
        }

        /// <summary>
        /// The tracing configuration is built as a fluent chain of nested `CSharpInvocationStatement`s
        /// (`trace.AddAspNetCoreInstrumentation().AddOtlpExporter(...)`, etc.) rather than a flat,
        /// insertable list - there is no public API to splice a call in at the *front* of an
        /// already-built chain like that (the intermediate nodes' `Expression`/`Reference` are
        /// construction-time only, not settable). Appending is possible though: the outermost node
        /// of that chain (whatever the last-configured instrumentation call happens to be) is a
        /// plain `CSharpStatement`, so calling `.AddInvocation(...)` on it produces one further
        /// wrapping call, and the reference sitting in the trace lambda's own (mutable) `Statements`
        /// list can simply be swapped for it. Ordering of `Add*`/`AddSource` calls has no functional
        /// effect on OpenTelemetry's `TracerProviderBuilder`, so appending instead of prepending is
        /// not a behavioural compromise here - see WolverineTelemetryConfiguratorExtension.cs for
        /// the full reasoning and the module-authoring report for why this differs from
        /// Intent.Eventing.MassTransit's abandoned attempt at the same problem.
        /// </summary>
        private static void AddWolverineTraceSource(CSharpFile file)
        {
            var priClass = file.Classes.First();
            var method = priClass.FindMethod("AddTelemetryConfiguration");
            if (method == null)
            {
                return;
            }

            if (method.FindStatement(stmt => stmt.HasMetadata("telemetry-tracing")) is not IHasCSharpStatementsActual tracingStatement)
            {
                // Capture Traces is disabled - there is no tracing configuration to add a source to.
                return;
            }

            if (tracingStatement.Statements.FirstOrDefault() is not CSharpLambdaBlock traceLambda ||
                traceLambda.Statements.Count == 0)
            {
                return;
            }

            var currentChain = traceLambda.Statements[0];
            traceLambda.Statements[0] = currentChain.AddInvocation("AddSource", inv => inv.AddArgument(@"""Wolverine"""));
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