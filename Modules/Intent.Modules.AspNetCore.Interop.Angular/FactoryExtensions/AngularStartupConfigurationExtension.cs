using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Interop.Angular.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class AngularStartupConfigurationExtension : FactoryExtensionBase
{
    public override string Id => "Intent.AspNetCore.Interop.Angular.AngularStartupConfigurationExtension";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
        template.AddNugetDependency(new NugetPackageInfo("Microsoft.TypeScript.MsBuild", "3.5.3"));

        if (template.OutputTarget.IsNetCore3App())
        {
            template.AddNugetDependency(new NugetPackageInfo("Microsoft.AspNetCore.SpaServices.Extensions", "3.1.4"));
        }

        template.CSharpFile.OnBuild(file =>
        {
            var startup = template.StartupFile;

            file.AddUsing("Microsoft.AspNetCore.SpaServices.AngularCli");

            startup.AddServiceConfiguration(ctx => $"ConfigureAngularSpa({ctx.Services});");
            startup.AddAppConfiguration(ctx => $"InitializeAngularSpa({ctx.App}, {ctx.Env});");

            startup.AddMethod("void", "ConfigureAngularSpa", method =>
            {
                method.AddParameter("IServiceCollection", "services");
                method.AddStatement($@"// In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {{
                configuration.RootPath = ""{GetAngularAppRelativePath(template.OutputTarget.Application)}/dist"";
            }});");
            });

            startup.AddMethod("void", "InitializeAngularSpa", method =>
            {
                method.AddParameter("IApplicationBuilder", "app");
                method.AddParameter(template.OutputTarget.IsNetCore2App() ? "IHostingEnvironment" : "IWebHostEnvironment", "env");
                method.AddStatement($@"app.UseSpa(spa =>
            {{
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = ""{GetAngularAppRelativePath(template.OutputTarget.Application)}"";

                if (env.IsDevelopment())
                {{
                    spa.UseAngularCliServer(npmScript: ""start"");
                }}
            }});");
            });
        }, 200);
    }

    private static IOutputTarget GetFrontEndOutputTarget(IApplication application)
    {
        return application.OutputTargets.FirstOrDefault(x => x.HasRole("Front End"));
    }

    private static string GetAngularAppRelativePath(IApplication application)
    {
        var outputTargetPath = GetFrontEndOutputTarget(application).GetTargetPath();
        return string.Join("/", outputTargetPath.Skip(1));
    }
}