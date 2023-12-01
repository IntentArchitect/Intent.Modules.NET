using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
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
                startup.AddAppConfiguration(ctx => $"{ctx.App}.UseSwashbuckle({ctx.Configuration});");
            }, 100);
        }
    }
}