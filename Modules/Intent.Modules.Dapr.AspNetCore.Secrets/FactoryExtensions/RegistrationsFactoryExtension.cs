using System;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Secrets.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RegistrationsFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.Secrets.RegistrationsFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            AddStartupRegistrations(application);
        }

        private void AddStartupRegistrations(IApplication application)
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

                configureMethod.InsertStatement(0, "app.AddDaprSecretStore(Configuration);");
            });
        }

        /*
        private void AddProgramRegistrations(IApplication application)
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

                var @class = file.Classes.First();

                string storeName = programTemplate.ExecutionContext.Settings.GetDapperSettings().SecretStoreName();
                var hostBuilder = @class.FindMethod("CreateHostBuilder");
                var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
                hostBuilderChain.Statements.Last()
                    .InsertAbove(new CSharpInvocationStatement("ConfigureAppConfiguration")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("(config)")
                    .AddStatement("var daprClient = new DaprClientBuilder().Build();")
                    .AddStatement($@"var secretDescriptors = new List<DaprSecretDescriptor>
                {{                   
{CreateInitStatement(programTemplate)}                    
                }};")
                    .AddStatement($"config.AddDaprSecretStore(\"{storeName}\", secretDescriptors, daprClient);")
                    ));
            }, 10);
        }

        private string CreateInitStatement(ICSharpFileBuilderTemplate programTemplate)
        {
            string[] secretDescriptors = programTemplate.ExecutionContext.Settings.GetDapperSettings().SecretDescriptors()?.Split(',');
            var initStatements = new StringBuilder();
            if (secretDescriptors != null && secretDescriptors.Length > 0)
            {
                foreach (var secretDescriptor in secretDescriptors)
                {
                    initStatements.AppendLine($"                    new DaprSecretDescriptor(\"{secretDescriptor}\"),");
                }
            }
            return initStatements.ToString();
        }
        */
    }
}