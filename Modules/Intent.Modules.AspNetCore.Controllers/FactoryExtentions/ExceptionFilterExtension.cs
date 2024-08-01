using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ExceptionFilterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.ExceptionFilterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallStartupControllerFilter(application);
        }

        private static void InstallStartupControllerFilter(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            template?.CSharpFile.OnBuild(_ =>
            {
                var startup = template.StartupFile;

                startup.AddServiceConfigurationLambda(
                    methodName: "AddControllers",
                    parameters: new[] { "opt" },
                    configure: (statement, lambda, context) =>
                    {
                        statement
                            .WithArgumentsOnNewLines() // To maintain formatting of existing files
                            .AddMetadata("configure-services-controllers-generic", true) // legacy
                            .AddMetadata("configure-services-controllers", "generic"); // easier to identify

                        lambda.AddStatement($"{context.Parameters[0]}.Filters.Add<{template.GetExceptionFilterName()}>();");
                    },
                    priority: -10_000_000);

                startup.AddUseEndpointsStatement(
                    create: context => $"{context.Endpoints}.MapControllers();",
                    configure: (s, _) => s.AddMetadata("configure-endpoints-controllers-generic", true));
            });
        }
    }
}