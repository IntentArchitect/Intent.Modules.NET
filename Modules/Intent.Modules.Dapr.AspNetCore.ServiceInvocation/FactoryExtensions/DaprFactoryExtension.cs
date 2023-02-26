using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DaprFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.ServiceInvocation.DaprFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.EventDispatcher.Publish(new AppSettingRegistrationRequest("DaprSidekick", new
            {
                Sidecar = new
                {
                    AppId = application.GetDaprApplicationName(application.Id)
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
                var configureMethod = file.Classes.First().FindMethod("Configure");
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

                var addControllersStatement = configureServicesMethod.FindStatement(x => x.ToString().Contains("services.AddControllers()"));
                addControllersStatement.InsertBelow("services.AddDaprSidekick(Configuration);");
                addControllersStatement.Replace("services.AddControllers().AddDapr();");
            });
        }
    }
}