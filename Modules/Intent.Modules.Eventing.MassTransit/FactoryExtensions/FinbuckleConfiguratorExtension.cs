using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FinbuckleConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.FinbuckleConfiguratorExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration"));
            bool finbuckleInstalled = template != null;
            if (!finbuckleInstalled) return;

            WireupMassTransitFilters(application);
            WireupFinbuckleTenancyStrategy(application);
        }

        private void WireupMassTransitFilters(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(MassTransitConfigurationTemplate.TemplateId));
            template?.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddMassTransitConfiguration");

                if (method.FindStatement(p => p.HasMetadata("configure-masstransit")) is not CSharpInvocationStatement configMassTransit)
                {
                    return;
                }

                var broker = ((IHasCSharpStatements)configMassTransit.Statements.First())
                    .Statements
                    .First(stmt => stmt.HasMetadata("message-broker")) as CSharpInvocationStatement;
                if (broker == null) return;

                var inMemoryOutboxStmt = ((IHasCSharpStatements)broker.Statements.First()).Statements.FirstOrDefault(p => p.HasMetadata("in-memory-outbox"));
                inMemoryOutboxStmt?.InsertAbove("cfg.UseMessageScope(context);");

                var brokerConfig = (IHasCSharpStatements)broker.Statements.First();
                brokerConfig.AddStatement($"cfg.UsePublishFilter(typeof({template.GetFinbucklePublishingFilterName()}<>), context);");
                brokerConfig.AddStatement($"cfg.UseConsumeFilter(typeof({template.GetFinbuckleConsumingFilterName()}<>), context);");
                brokerConfig.AddStatement($"cfg.UseSendFilter(typeof({template.GetFinbuckleSendingFilterName()}<>), context);");
            });
        }

        private void WireupFinbuckleTenancyStrategy(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration"));

            template?.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("ConfigureMultiTenancy");

                if (method.FindStatement(p => p.HasMetadata("add-multi-tenant")) is not CSharpMethodChainStatement configFinbuckle)
                {
                    throw new Exception("Can't find add-multi-tenant meta data");
                }

                var firstStrategy = configFinbuckle.Statements.First(stmt => stmt.GetText("").Contains("Strategy("));
                firstStrategy.InsertAbove($"WithStrategy<{template.GetFinbuckleMessageHeaderStrategyName()}>(ServiceLifetime.Scoped)");
            });
        }
    }
}