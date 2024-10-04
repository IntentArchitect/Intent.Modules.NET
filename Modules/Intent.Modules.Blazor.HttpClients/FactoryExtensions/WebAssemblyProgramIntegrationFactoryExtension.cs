using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.HttpClients.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WebAssemblyProgramIntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.HttpClients.WebAssemblyProgramIntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            RegisterHttpClients(application);
        }

        private void RegisterHttpClients(IApplication application)
        {

            // NEW PATTERN WITH THE SHARED COMMON DependencyInjection CLASS:
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Blazor.Client.DependencyInjection);
            if (template is not null)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    template.UseType(template.GetHttpClientConfigurationName());

                    var @class = file.Classes.First();
                    var mainMethod = @class.FindMethod(x => x.ReturnType == "IServiceCollection");
                    if (mainMethod == null)
                        return;
                    mainMethod.AddStatement("services.AddHttpClients(configuration);");
                });
            }
            else
            {
                // FOR BACKWARD COMPATIBILITY WITH OLD WEBASSEMBLY MODULES:
                template = application.FindTemplateInstance<IProgramTemplate>(TemplateRoles.Blazor.WebAssembly.Program);
                if (template is null)
                {
                    return;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    template.UseType(template.GetHttpClientConfigurationName());

                    var @class = file.Classes.First();
                    var mainMethod = @class.FindMethod("Main");
                    if (mainMethod == null)
                        return;
                    var lastStatement = mainMethod.FindStatement(s => s.HasMetadata("run-builder"));
                    if (lastStatement == null)
                        return;
                    lastStatement.InsertAbove("builder.Services.AddHttpClients(builder.Configuration);");
                });
            }
            //var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            //startup?.AddNugetDependency(new NugetPackageInfo("Microsoft.AspNetCore.Components.WebAssembly.Server", "8.0.3"));

            //startup?.CSharpFile.AfterBuild(file =>
            //{
            //    startup.StartupFile.ConfigureServices((statements, context) =>
            //    {
            //        // TODO: Firstly, this is a hack, but the service interfaces need to be implemented for InteractiveAuto to work:
            //        startup.UseType(startup.GetHttpClientConfigurationName());
            //        statements.AddStatement($"{context.Services}.AddHttpClients({context.Configuration});", s => s.SeparatedFromPrevious());
            //    });
            //});
        }
    }
}