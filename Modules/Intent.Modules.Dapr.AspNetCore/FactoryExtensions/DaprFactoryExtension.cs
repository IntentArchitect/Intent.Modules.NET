using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DaprFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.DaprFactoryExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.EventDispatcher.Publish(LaunchProfileHttpPortRequired.EventId, new Dictionary<string, string>());
            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("DaprSidekick", new
            {
                Sidecar = new
                {
                    AppId = application.GetDaprApplicationName(application.Id),
                    ComponentsDirectory = "../dapr/components",
                    ConfigFile = "../dapr/config.yaml"
                }
            }));

            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate == null)
            {
                return;
            }

            var startup = startupTemplate.StartupFile;

            startupTemplate.AddNugetDependency(NugetPackages.ManDaprSidekickAspNetCore(startupTemplate.OutputTarget));
            startupTemplate.CSharpFile.AfterBuild(_ =>
            {
                startup.ConfigureServices((statements, context) =>
                {
                    if (statements.FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not CSharpInvocationStatement controllersStatement)
                    {
                        return;
                    }

                    controllersStatement.WithoutSemicolon();
                    controllersStatement.InsertBelow(new CSharpInvocationStatement(".AddDapr"));
                    statements.FindStatement(p => p.GetText(string.Empty).Contains("AddDapr"))
                        ?.InsertBelow(new CSharpInvocationStatement($"{context.Services}.AddDaprSidekick")
                            .AddArgument(context.Configuration));
                });

                startup.ConfigureApp((statements, _) =>
                {
                    statements
                        .FindStatement(x => x.ToString()!.Contains(".UseHttpsRedirection()"))?
                        .Remove();
                });
            }, 11);
        }
    }
}