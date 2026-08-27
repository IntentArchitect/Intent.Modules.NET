using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineEventingConfiguration;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantMiddleware;
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
        /// </summary>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var finbuckleInstalled = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration")) != null;
            if (!finbuckleInstalled)
            {
                return;
            }

            var middlewareTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineTenantMiddlewareTemplate.TemplateId);
            if (middlewareTemplate == null)
            {
                return;
            }

            var configurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineEventingConfigurationTemplate.TemplateId);
            if (configurationTemplate == null)
            {
                return;
            }

            var transport = application.Settings.GetWolverineMessageBusSettings().Transport().AsEnum();
            var configureMethodName = WolverineEventingConfigurationTemplate.GetConfigureMethodName(transport);

            configurationTemplate.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod(configureMethodName);
                if (method == null)
                {
                    return;
                }

                var middlewareTypeName = configurationTemplate.GetTypeName(WolverineTenantMiddlewareTemplate.TemplateId);
                method.AddStatement($"opts.Policies.AddMiddleware(typeof({middlewareTypeName}));", s => s.SeparatedFromPrevious());
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