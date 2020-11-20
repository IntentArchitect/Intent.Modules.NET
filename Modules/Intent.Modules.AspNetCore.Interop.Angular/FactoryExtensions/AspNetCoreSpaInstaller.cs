using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Interop.Angular.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class AspNetCoreSpaInstaller : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "AspNetCore.Interop.Angular.AspNetCoreSpaInstaller";
        public override int Order => 0;

        [IntentManaged(Mode.Ignore)]
        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                RequestInitialization(application);
            }
        }

        private void RequestInitialization(IApplication application)
        {
            application.EventDispatcher.Publish(ServiceConfigurationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { ServiceConfigurationRequiredEvent.UsingsKey, $@"Microsoft.AspNetCore.SpaServices.AngularCli;" },
                { ServiceConfigurationRequiredEvent.CallKey, "ConfigureAngularSpa(services);" },
                { ServiceConfigurationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void ConfigureAngularSpa(IServiceCollection services)
        {{
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {{
                configuration.RootPath = ""ClientApp/dist"";
            }});
        }}" }
            });

            application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"Microsoft.AspNetCore.SpaServices.AngularCli;" },
                { InitializationRequiredEvent.CallKey, $@"InitializeAngularSpa(app, env);" },
                { InitializationRequiredEvent.MethodKey, $@"
        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void InitializeAngularSpa(IApplicationBuilder app, { (GetWebCoreProject(application).IsNetCore2App() ? "IHostingEnvironment" : "IWebHostEnvironment") } env)
        {{
            app.UseSpa(spa =>
            {{
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = ""ClientApp"";

                if (env.IsDevelopment())
                {{
                    spa.UseAngularCliServer(npmScript: ""start"");
                }}
            }});
        }}" }
            });
        }

        public static IOutputTarget GetWebCoreProject(IApplication application)
        {
            return application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.CoreWebApp);
        }
    }
}