using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
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
            AddProgramRegistrations(application);
            AddStartupRegistrations(application);
        }

        private void AddProgramRegistrations(IApplication application)
        {
            var startupTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup");
            if (startupTemplate == null)
            {
                return;
            }

            startupTemplate.AddNugetDependency(NuGetPackages.DaprExtensionsConfiguration);

            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var configureMethod = @class.FindMethod("Configure");
                if (configureMethod == null)
                {
                    return;
                }
                startupTemplate.GetDaprConfigurationConfigurationName();
                configureMethod.InsertStatement(0, "app.LoadDaprConfigurationStoreDeferred(Configuration);");
            }, 15);
        }

        private void AddStartupRegistrations(IApplication application)
        {
            var programTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Program");
            if (programTemplate == null)
            {
                return;
            }

            programTemplate.AddNugetDependency(NuGetPackages.DaprExtensionsConfiguration);

            programTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Dapr.Client");
                file.AddUsing("Dapr.Extensions.Configuration");
                file.AddUsing("System.Collections.Generic");

                programTemplate.GetDaprConfigurationConfigurationName();
                var @class = file.Classes.First();
                var hostBuilder = @class.FindMethod("CreateHostBuilder");
                var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
                var appConfigurationBlock = (CSharpInvocationStatement)hostBuilderChain.FindStatement(stmt => stmt.GetText("").StartsWith("ConfigureAppConfiguration"));
                if (appConfigurationBlock == null)
                {
                    appConfigurationBlock = new CSharpInvocationStatement("ConfigureAppConfiguration")
                        .WithoutSemicolon()
                        .AddArgument(new CSharpLambdaBlock("(config)")
                        .AddStatement("config.AddDaprConfigurationStoreDeferred();"));
                    hostBuilderChain.Statements.Last()
                        .InsertAbove(appConfigurationBlock);
                }
                else
                {
                    ((CSharpLambdaBlock)appConfigurationBlock.Statements[0]).Statements.Add("config.AddDaprConfigurationStoreDeferred();");
                }
            }, 10);
        }
    }
}