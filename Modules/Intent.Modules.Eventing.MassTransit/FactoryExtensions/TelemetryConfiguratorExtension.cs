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
public class TelemetryConfiguratorExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.TelemetryConfiguratorExtension";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        UpdateOpenTelemetryConfiguration(application);
    }

    private void UpdateOpenTelemetryConfiguration(IApplication application)
    {
        // Until we can modify wrapped invocation statements we can't go with this solution.
        // See the TEMP FIX in OpenTelemetryConfigurationTemplate
        
        //var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.OpenTelemetry"));
        
        // template?.CSharpFile.AfterBuild(file =>
        // {
        //     var priClass = file.Classes.First();
        //     var method = priClass.FindMethod("AddTelemetryConfiguration");
            // var telemetryConfigStmt = (CSharpMethodChainStatement)method.FindStatement(stmt => stmt.HasMetadata("telemetry-config"));
            // var telemetryTranceStmt = (CSharpInvocationStatement)telemetryConfigStmt.FindStatement(stmt => stmt.HasMetadata("telemetry-tracing"));
            // var traceChain = (CSharpMethodChainStatement)((CSharpLambdaBlock)telemetryTranceStmt.Statements.First()).Statements.First();

            //traceChain.Statements.Insert(0, @"AddSource(""MassTransit"")");
        //});
    }

}