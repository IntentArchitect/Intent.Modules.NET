using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Interop.Angular.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AngularStartupDecorator : StartupDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Interop.Angular.AngularStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AngularStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency("Microsoft.TypeScript.MsBuild", "3.5.3");
            if (_template.OutputTarget.IsNetCore3App())
            {
                _template.AddNugetDependency("Microsoft.AspNetCore.SpaServices.Extensions", "3.1.4");
            }
        }

        public override int Priority => 200;

        public override string ConfigureServices()
        {
            return @"ConfigureAngularSpa(services);";
        }

        public override string Configuration()
        {
            return @"InitializeAngularSpa(app, env);";
        }

        public override string Methods()
        {
            return $@"        
        private void ConfigureAngularSpa(IServiceCollection services)
        {{
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {{
                configuration.RootPath = ""{GetAngularAppRelativePath(_template.OutputTarget.Application)}/dist"";
            }});
        }}

        private void InitializeAngularSpa(IApplicationBuilder app, {(_template.OutputTarget.IsNetCore2App() ? "IHostingEnvironment" : "IWebHostEnvironment")} env)
        {{
            app.UseSpa(spa =>
            {{
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = ""{GetAngularAppRelativePath(_template.OutputTarget.Application)}"";

                if (env.IsDevelopment())
                {{
                    spa.UseAngularCliServer(npmScript: ""start"");
                }}
            }});
        }}";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.AspNetCore.SpaServices.AngularCli";
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
}