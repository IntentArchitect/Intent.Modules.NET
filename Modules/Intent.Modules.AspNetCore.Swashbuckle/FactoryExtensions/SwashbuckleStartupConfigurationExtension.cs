using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwashbuckleStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Swashbuckle.SwashbuckleStartupConfigurationExtension";
        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            var configurationTemplate = template.OutputTarget.FindTemplateInstance<IClassProvider>(SwashbuckleConfigurationTemplate.TemplateId);
            ((IntentTemplateBase)template).AddTemplateDependency(SwashbuckleConfigurationTemplate.TemplateId);
            template.AddUsing(configurationTemplate.Namespace);

            ((IntentTemplateBase)template).PublishDefaultLaunchUrlPathRequest("swagger");

            template.CSharpFile.OnBuild(_ =>
            {
                var startup = template.StartupFile;
                startup.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureSwagger({ctx.Configuration});");

                var useSwashbuckleStatement = "{0}.UseSwashbuckle({1});";

                startup.ConfigureApp((statements, ctx) =>
                {
                    var statementToAdd = string.Format(useSwashbuckleStatement, ctx.App, ctx.Configuration);

                    // The swagger UI endpoint doesn't work if it is placed after UseEndpoints is called and MVC is set up.
                    // We do it conditionally to minimize SF noise after an update for clients which aren't using MVC.
                    var useEndpointStatement = statements.FindStatement(x => x.ToString().Contains("UseEndpoints"));
                    if (useEndpointStatement != null &&
                        application.GetInstalledModules().Any(x => string.Equals(x.ModuleId, "Intent.AspNetCore.Mvc")))
                    {
                        useEndpointStatement.InsertAbove(statementToAdd);
                    }
                    else
                    {
                        startup.AddAppConfiguration(context => string.Format(useSwashbuckleStatement, context.App, context.Configuration));
                    }
                });
            }, 100);
        }
    }
}