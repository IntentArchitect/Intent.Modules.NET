using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Plugins;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityServerStartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.IdentityServer4.SecureTokenServer.IdentityServerStartupExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

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
            var template = application.FindTemplateInstance<IdentityServerConfigurationTemplate>(IdentityServerConfigurationTemplate.TemplateId);
            if (template is null)
            {
                return;
            }

            application.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureIdentityServer", ServiceConfigurationRequest.ParameterType.Configuration)
                .WithPriority(-9)
                .HasDependency(template));

            application.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseIdentityServer"));

            template.AddNugetDependency(NugetPackages.IdentityServer4);
        }
    }
}