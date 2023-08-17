using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
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
            application.EventDispatcher.Publish(LaunchProfileHttpPortRequired.EventId);
            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("DaprSidekick", new
            {
                Sidecar = new
                {
                    AppId = application.GetDaprApplicationName(application.Id),
                    ComponentsDirectory = "../dapr/components",
                    ConfigFile = "../dapr/config.yaml"
                }
            }));

            var startupTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup");
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(NuGetPackages.ManDaprSidekickAspNetCore);
            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var configureMethod = priClass.FindMethod("Configure");
                if (configureMethod == null)
                {
                    return;
                }

                configureMethod
                    .FindStatement(x => x.ToString().Contains("app.UseHttpsRedirection()"))?
                    .Remove();

                var configureServicesMethod = file.Classes.First().FindMethod("ConfigureServices");
                if (configureServicesMethod == null)
                {
                    return;
                }

                if (priClass.FindMethod("ConfigureServices")
                        .FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not
                    CSharpInvocationStatement controllersStatement)
                {
                    return;
                }

                controllersStatement.WithoutSemicolon();
                controllersStatement.InsertBelow(new CSharpInvocationStatement(".AddDapr"));
                configureServicesMethod.FindStatement(p => p.GetText(string.Empty).Contains("AddDapr"))
                    ?.InsertBelow(new CSharpInvocationStatement("services.AddDaprSidekick")
                        .AddArgument("Configuration"));
            }, 11);
        }
    }
}