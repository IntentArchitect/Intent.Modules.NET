using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantMiddleware;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantStrategy;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineFinbuckleConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Wolverine.WolverineFinbuckleConfiguratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// R12.2: registers <see cref="WolverineTenantMiddlewareTemplate"/> once, at Wolverine
        /// Host Configuration scope (`opts.Policies.AddMiddleware(Type)` applies it to every
        /// listener/handler), never per-message. Only wired up when Finbuckle multi-tenancy is
        /// installed - when it is not, the middleware template does not exist (its own
        /// CanRunTemplate is gated the same way) and this method exits before referencing its
        /// generated type name, so the type is never referenced without being emitted.
        /// The generated middleware class is static (Wolverine's static-middleware convention -
        /// no DI overhead), so the generic `AddMiddleware&lt;T&gt;()` overload cannot be used
        /// (`CS0718: static types cannot be used as type arguments`); the `Type`-based overload
        /// exists in Wolverine's own API for exactly this case.
        /// <para>
        /// Targets Intent.Wolverine.Common's shared WolverineConfiguration template, specifically
        /// this module's own ConfigureEventing method there (a stable name regardless of transport,
        /// unlike the retired per-transport Configure{Transport} methods) - see that module's
        /// CONTEXT.md and this module's ContributeEventingConfiguration for the contribution
        /// mechanism this rides on.
        /// </para>
        /// </summary>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var finbuckleInstalled = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration")) != null;
            if (!finbuckleInstalled)
            {
                return;
            }

            WireupWolverineTenancyMiddleware(application);
            WireupWolverineTenancyStrategy(application);
        }

        private static void WireupWolverineTenancyMiddleware(IApplication application)
        {
            var middlewareTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineTenantMiddlewareTemplate.TemplateId);
            if (middlewareTemplate == null)
            {
                return;
            }

            var configurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Wolverine.Common.WolverineConfiguration");
            if (configurationTemplate == null)
            {
                return;
            }

            configurationTemplate.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("ConfigureEventing");
                if (method == null)
                {
                    return;
                }

                var middlewareTypeName = configurationTemplate.GetTypeName(WolverineTenantMiddlewareTemplate.TemplateId);
                method.AddStatement($"opts.Policies.AddMiddleware(typeof({middlewareTypeName}));", s => s.SeparatedFromPrevious());
            });
        }

        /// <summary>
        /// R12.2: registers <see cref="WolverineTenantStrategyTemplate"/> into the FOREIGN
        /// MultiTenancyConfiguration chain (owned by Intent.Modules.AspNetCore.MultiTenancy), not
        /// into this module's own WolverineConfiguration contribution - Finbuckle's strategy list
        /// lives on that chain and nowhere else. Same find-template + OnBuild + InsertAbove idiom
        /// Intent.Eventing.MassTransit's FinbuckleConfiguratorExtension already uses for
        /// FinbuckleMessageHeaderStrategy.
        /// <para>
        /// InsertAbove is not cosmetic: the foreign module always terminates the chain with the
        /// HTTP header strategy carrying the trailing semicolon
        /// (`.WithHeaderStrategy("X-Tenant-Identifier");`), so appending would land this call after
        /// a semicolon and fail to compile. The match on the first statement whose text contains
        /// "Strategy(" is the same textual match the MassTransit reference uses, and holds across
        /// every store/strategy combination the foreign module can emit.
        /// </para>
        /// </summary>
        private static void WireupWolverineTenancyStrategy(IApplication application)
        {
            var strategyTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineTenantStrategyTemplate.TemplateId);
            if (strategyTemplate == null)
            {
                return;
            }

            var multiTenancyTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration"));
            if (multiTenancyTemplate == null)
            {
                return;
            }

            multiTenancyTemplate.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("ConfigureMultiTenancy");
                if (method?.FindStatement(p => p.HasMetadata("add-multi-tenant")) is not CSharpMethodChainStatement configFinbuckle)
                {
                    return;
                }

                var firstStrategy = configFinbuckle.Statements.FirstOrDefault(stmt => stmt.GetText("").Contains("Strategy("));
                if (firstStrategy == null)
                {
                    return;
                }

                var strategyTypeName = multiTenancyTemplate.GetTypeName(WolverineTenantStrategyTemplate.TemplateId);
                firstStrategy.InsertAbove($"WithStrategy<{strategyTypeName}>(ServiceLifetime.Scoped)");
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
