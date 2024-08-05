using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EFOutboxPatternConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.EntityFrameworkCore.EFOutboxPatternConfiguratorExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework())
            {
                return;
            }

            var template =
                application.FindTemplateInstance<MassTransitConfigurationTemplate>(
                    TemplateDependency.OnTemplate(MassTransitConfigurationTemplate.TemplateId));
            if (template == null)
            {
                return;
            }

            var provider = application.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum();
            switch (provider)
            {
                default:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    Logging.Log.Warning(
                        $@"Database Provider {provider} is not supported for Entity Framework outbox pattern.");
                    template.AddNugetDependency(NugetPackages.MassTransitEntityFrameworkCore(template.OutputTarget));
                    AddEntityFrameworkOutboxStatement(template, conf => conf
                        .AddStatement("o.UseBusOutbox();"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    template.AddNugetDependency(NugetPackages.MassTransitEntityFrameworkCore(template.OutputTarget));
                    AddEntityFrameworkOutboxStatement(template, conf => conf
                        .AddStatement("o.UseSqlServer();")
                        .AddStatement("o.UseBusOutbox();"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    template.AddNugetDependency(NugetPackages.MassTransitEntityFrameworkCore(template.OutputTarget));
                    AddEntityFrameworkOutboxStatement(template, conf => conf
                        .AddStatement("o.UsePostgres();")
                        .AddStatement("o.UseBusOutbox();"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                    template.AddNugetDependency(NugetPackages.MassTransitEntityFrameworkCore(template.OutputTarget));
                    AddEntityFrameworkOutboxStatement(template, conf => conf
                        .AddStatement("o.UseMySql();")
                        .AddStatement("o.UseBusOutbox();"));
                    break;
            }
        }

        private static void AddEntityFrameworkOutboxStatement(
            ICSharpFileBuilderTemplate template,
            Action<CSharpLambdaBlock> outboxConfigBlock)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("AddMassTransitConfiguration");

                if (method.FindStatement(p => p.HasMetadata("configure-masstransit")) is not CSharpInvocationStatement configMassTransit) { return; }
                if (configMassTransit.Statements.First() is not CSharpLambdaBlock configMassTransitBlock) { return; }

                CSharpLambdaBlock outboxConfigLambda;
                if (configMassTransitBlock.FindStatement(p => p.HasMetadata("outbox-config")) is not CSharpInvocationStatement outboxConfigInvocation)
                {
                    outboxConfigLambda = new CSharpLambdaBlock("o");
                    configMassTransitBlock.AddStatement(
                        new CSharpInvocationStatement($@"x.AddEntityFrameworkOutbox<{template.GetDbContextName()}>")
                            .AddArgument(outboxConfigLambda)
                            .AddMetadata("outbox-config", true));
                }
                else
                {
                    outboxConfigLambda = outboxConfigInvocation.Statements.First() as CSharpLambdaBlock;
                }

                if (outboxConfigLambda is not null)
                {
                    outboxConfigBlock(outboxConfigLambda);
                }
            });
        }
    }
}