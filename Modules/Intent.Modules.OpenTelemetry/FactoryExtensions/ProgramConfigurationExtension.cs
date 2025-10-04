using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
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
            if (!application.Settings.GetOpenTelemetry().CaptureLogs() || application.Settings.GetOpenTelemetry().Export().IsAzureMonitorOpentelemetryDistro())
            {
                return;
            }


            var programTemplates = application.FindTemplateInstances<IProgramTemplate>("App.Program");
            if (programTemplates == null || !programTemplates.Any())
            {
                return;
            }

            var configTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(OpenTelemetryConfigurationTemplate.TemplateId);
            if (configTemplates == null || !configTemplates.Any())
            {
                return;
            }

            if (programTemplates.Count() == 1 && configTemplates.Count() == 1)
            {
                UpdateProgramFile(programTemplates.First(), configTemplates.First());
            }
            else
            {
                var matches = programTemplates.Join(
                    configTemplates,
                    programTemplate => programTemplate.OutputTarget.GetProject().Id,
                    configTemplate => configTemplate.OutputTarget.GetProject().Id,
                    (ProgramTemplate, ConfigTemplate) => (ProgramTemplate, ConfigTemplate));

                foreach (var match in matches)
                {
                    UpdateProgramFile(match.ProgramTemplate, match.ConfigTemplate);
                }
            }
        }

        private static void UpdateProgramFile(IProgramTemplate programTemplate, ICSharpFileBuilderTemplate configTemplate)
        {
            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing(configTemplate.Namespace);
                file.AddUsing("Microsoft.Extensions.Configuration");
                file.AddUsing("Microsoft.Extensions.Logging");

                programTemplate.ProgramFile.ConfigureMainStatementsBlock(statements =>
                {
                    if (!programTemplate.ProgramFile.UsesMinimalHostingModel)
                    {
                        var @class = file.Classes.First();
                        var hostBuilder = @class.FindMethod("CreateHostBuilder");
                        var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
                        (hostBuilderChain.FindStatement(p => p.ToString()!.Contains("UseSerilog")) as CSharpInvocationStatement)?
                            .AddArgument("writeToProviders: true");
                    }
                    else
                    {
                        (statements.FindStatement(p => p.ToString()!.Contains("UseSerilog")) as CSharpInvocationStatement)?
                            .AddArgument("writeToProviders: true");
                    }

                    programTemplate.ProgramFile.ConfigureAppLogging(true, (block, parameters) =>
                    {
                        block.AddStatement($"{parameters[1]}.ClearProviders();");
                        block.AddStatement($"{parameters[1]}.AddTelemetryConfiguration({parameters[0]});");
                    });
                });
            }, 30);
        }
    }
}