using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Dapr.AspNetCore.Configuration.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Configuration.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegistrationsFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.Configuration.RegistrationsFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            AddStartupRegistrations(application);
            AddProgramRegistrations(application);
        }

        private static void AddStartupRegistrations(IApplication application)
        {
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(NugetPackages.DaprExtensionsConfiguration(startupTemplate.OutputTarget));
            startupTemplate.CSharpFile.AfterBuild(_ =>
            {
                startupTemplate.StartupFile.ConfigureApp((hasCSharpStatements, ctx) =>
                {
                    startupTemplate.GetDaprConfigurationConfigurationName();
                    hasCSharpStatements.InsertStatement(0, $"{ctx.App}.LoadDaprConfigurationStoreDeferred({ctx.Configuration});");
                });
            }, 15);
        }

        private static void AddProgramRegistrations(IApplication application)
        {
            var programTemplate = application.FindTemplateInstance<IProgramTemplate>("App.Program");
            if (programTemplate == null)
            {
                return;
            }

            programTemplate.AddNugetDependency(NugetPackages.DaprExtensionsConfiguration(programTemplate.OutputTarget));

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Dapr.Client");
                file.AddUsing("Dapr.Extensions.Configuration");
                file.AddUsing("System.Collections.Generic");

                programTemplate.GetDaprConfigurationConfigurationName();
                programTemplate.ProgramFile.ConfigureAppConfiguration(false, (statements, parameters) =>
                {
                    statements.AddStatement($"{parameters[^1]}.AddDaprConfigurationStoreDeferred();");
                });
            }, 12);
        }
    }
}