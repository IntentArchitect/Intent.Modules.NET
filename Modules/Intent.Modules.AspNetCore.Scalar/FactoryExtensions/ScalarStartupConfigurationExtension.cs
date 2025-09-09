using System.Collections.Generic;
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

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            try
            {
                var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
                ((IntentTemplateBase)template).PublishDefaultLaunchUrlPathRequest("scalar/v1");
            }
            catch (System.Exception)
            {

            }
            
            base.OnBeforeTemplateExecution(application);
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            //ASP Net Core Versioning
            DisableTemplate(application, "Intent.AspNetCore.Versioning.ApiVersionSwaggerGenOptions");

            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            var configurationTemplate = template.OutputTarget.FindTemplateInstance<IClassProvider>(OpenApiConfigurationTemplate.TemplateId);
            ((IntentTemplateBase)template).AddTemplateDependency(OpenApiConfigurationTemplate.TemplateId);
            template.AddUsing(configurationTemplate.Namespace);

            try
            {
                ((IntentTemplateBase)template).PublishDefaultLaunchUrlPathRequest("scalar/v1");
            }
            catch (System.Exception)
            {
            }

            template.CSharpFile.AfterBuild(_ =>
            {
                var startup = template.StartupFile;

                template.AddUsing("Scalar.AspNetCore");

                startup.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureOpenApi();");

                startup.ConfigureEndpoints((statements, context) =>
                {
                    statements.InsertStatement(0, (CSharpStatement)$"{context.Endpoints}.MapOpenApi();");
                    statements.InsertStatement(0, (CSharpStatement)$"{context.Endpoints}.MapScalarApiReference();");
                });
            }, 999);
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