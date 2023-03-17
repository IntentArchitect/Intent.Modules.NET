using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class TelemetryConfigurator : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.TelemetryConfigurator";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        if (application.InstalledModules.All(p => p.ModuleId != "Intent.OpenTelemetry"))
        {
            return;
        }

        UpdateIntegrationEventMessage(application);
        UpdateOpenTelemetryConfiguration(application);
    }

    private void UpdateOpenTelemetryConfiguration(IApplication application)
    { 
        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.OpenTelemetry"));
        if (template == null)
        {
            return;
        }

        template.CSharpFile.AfterBuild(file =>
        {
            var priClass = file.Classes.First();
            var method = priClass.FindMethod("AddTelemetryConfiguration");
            var telemConfigStmt = (CSharpMethodChainStatement)method.FindStatement(stmt => stmt.HasMetadata("telemetry-config"));
            var telemTranceStmt = (CSharpInvocationStatement)telemConfigStmt.FindStatement(stmt => stmt.HasMetadata("telemetry-tracing"));
            var traceChain = (CSharpMethodChainStatement)((CSharpLambdaBlock)telemTranceStmt.Statements.First()).Statements.First();
            
            traceChain.Statements.Insert(0, @"AddSource(""MassTransit"")");
        });
    }

    private static void UpdateIntegrationEventMessage(IApplication application)
    {
        var templates = application.FindTemplateInstances<IntegrationEventMessageTemplate>(TemplateDependency.OnTemplate(IntegrationEventMessageTemplate.TemplateId));
        foreach (var template in templates)
        {
            template.CSharpFile.AfterBuild(file =>
            {
                var rec = file.Records.First();
                rec.Properties.Insert(0, new CSharpProperty(template.UseType("System.Guid") + "?", "CorrelationId", rec).Init());
            }, 1000);
        }
    }
}