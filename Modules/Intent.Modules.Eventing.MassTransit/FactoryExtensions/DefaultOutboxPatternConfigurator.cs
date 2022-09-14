using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DefaultOutboxPatternConfigurator : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.DefaultOutboxPatternConfigurator";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            if (application.Settings.GetEventingSettings().OutboxPattern().IsNone())
            {
                return;
            }

            var template = application.FindTemplateInstance<MassTransitConfigurationTemplate>(TemplateDependency.OnTemplate(MassTransitConfigurationTemplate.TemplateId));
            if (template == null)
            {
                return;
            }

            if (application.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
            {
                template.MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Add("cfg.UseInMemoryOutbox();");
                return;
            }
            
            if (application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework())
            {
                template.MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Add("#warning Please install Intent.Eventing.MassTransit.EntityFrameworkCore module for the Outbox pattern to persist to the database");
                return;
            }
        }
    }
}