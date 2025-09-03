using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Scalar.Templates.OpenApiConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ScalarStartupConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Scalar.ScalarStartupConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //ASP Net Core Versioning
            DisableTemplate(application, "Intent.AspNetCore.Versioning.ApiVersionSwaggerGenOptions");

            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            var configurationTemplate = template.OutputTarget.FindTemplateInstance<IClassProvider>(OpenApiConfigurationTemplate.TemplateId);
            ((IntentTemplateBase)template).AddTemplateDependency(OpenApiConfigurationTemplate.TemplateId);
            template.AddUsing(configurationTemplate.Namespace);

            ((IntentTemplateBase)template).PublishDefaultLaunchUrlPathRequest("scalar/v1");

            template.CSharpFile.OnBuild(_ =>
            {
                var startup = template.StartupFile;

                template.AddUsing("Scalar.AspNetCore");

                startup.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureOpenApi();");

                var mapOpenApiStatement = "{0}.MapOpenApi();";
                var mapScalarStatement = "{0}.MapScalarApiReference();";

                startup.ConfigureApp((statements, ctx) =>
                {
                    var statementToAdd1 = string.Format(mapOpenApiStatement, ctx.App);
                    var statementToAdd2 = string.Format(mapScalarStatement, ctx.App);

                    // The swagger UI endpoint doesn't work if it is placed after UseEndpoints is called and MVC is set up.
                    // We do it conditionally to minimize SF noise after an update for clients which aren't using MVC.
                    var useEndpointStatement = statements.FindStatement(x => x.ToString().Contains("UseEndpoints"));
                    if (useEndpointStatement != null &&
                        application.GetInstalledModules().Any(x => string.Equals(x.ModuleId, "Intent.AspNetCore.Mvc")))
                    {
                        useEndpointStatement.InsertAbove(statementToAdd1);
                        useEndpointStatement.InsertAbove(statementToAdd2);
                    }
                    else
                    {
                        startup.AddAppConfiguration(context => string.Format(mapOpenApiStatement, context.App));
                        startup.AddAppConfiguration(context => string.Format(mapScalarStatement, context.App));
                    }
                });
            }, 100);
        }

        private void DisableTemplate(IApplication application, string templateId)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(templateId);

            if (template is not null)
            {
                template.CanRun = false;
            }

        }
    }
}