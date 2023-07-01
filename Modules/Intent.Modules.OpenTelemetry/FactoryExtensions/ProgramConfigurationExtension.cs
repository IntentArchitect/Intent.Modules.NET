using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.OpenTelemetry.Settings;
using Intent.Modules.OpenTelemetry.Templates.OpenTelemetryConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.OpenTelemetry.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProgramConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.OpenTelemetry.ProgramConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var programTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("App.Program"));
            if (programTemplate == null)
            {
                return;
            }

            var configTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(OpenTelemetryConfigurationTemplate.TemplateId));
            if (configTemplate == null)
            {
                return;
            }

            if (!application.Settings.GetOpenTelemetry().CaptureLogs())
            {
                return;
            }

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(configTemplate.Namespace);
                file.AddUsing("Microsoft.Extensions.Configuration");
                file.AddUsing("Microsoft.Extensions.Logging");
        
                var @class = file.Classes.First();
                var hostBuilder = @class.FindMethod("CreateHostBuilder");
                var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
                (hostBuilderChain.FindStatement(p => p.ToString().Contains("UseSerilog")) as CSharpInvocationStatement)
                    ?.AddArgument("writeToProviders: true");
                hostBuilderChain.Statements.Last().InsertAbove(new CSharpInvocationStatement("ConfigureLogging")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("(ctx, logBuilder)")
                        .AddStatements($@"
                        logBuilder.ClearProviders();
                        logBuilder.AddTelemetryConfiguration(ctx);"))); 
            }, 30);
        }
    }
}