using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.OutboxPersistence.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MessageProviderSpecificCodeManipulator : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.OutboxPersistence.MessageProviderSpecificCodeManipulator";

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
            var template = application.FindTemplateInstance<MassTransitConfigurationTemplate>(TemplateDependency.OnTemplate(MassTransitConfigurationTemplate.TemplateId));
            if (template == null)
            {
                return;
            }

            if (application.Settings.GetEventingSettings().MessagingServiceProvider().IsInMemory())
            {
                template.MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Add("cfg.UseInMemoryOutbox();");
                return;
            }

            switch (application.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                // Assume In-Memory outbox pattern when an unsupported (or in-memory) option is selected
                default:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    template.MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Add("cfg.UseInMemoryOutbox();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    template.AddNugetDependency(NuGetPackages.MassTransitEntityFrameworkCore);
                    template.AdditionalConfiguration.Add(new MassTransitConfigurationTemplate.ScopedExtensionMethodConfiguration($"AddEntityFrameworkOutbox<{template.GetDbContextName()}>", "o")
                        .AppendNestedLines(new[]
                        {
                            "o.UseSqlServer();",
                            "o.UseBusOutbox();"
                        }));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    template.AddNugetDependency(NuGetPackages.MassTransitEntityFrameworkCore);
                    template.AdditionalConfiguration.Add(new MassTransitConfigurationTemplate.ScopedExtensionMethodConfiguration($"AddEntityFrameworkOutbox<{template.GetDbContextName()}>", "o")
                        .AppendNestedLines(new[]
                        {
                            "o.UsePostgres();",
                            "o.UseBusOutbox();"
                        }));
                    break;
            }
        }
    }
}