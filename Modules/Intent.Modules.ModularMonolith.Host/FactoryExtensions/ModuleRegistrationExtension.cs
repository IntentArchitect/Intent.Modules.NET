using System;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.ModularMonolith.Host.Templates;
using Intent.Modules.ModularMonolith.Host.Templates.ModuleInstallerInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ModuleRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.ModuleRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            RegisterStartup(application);
            AdjustInfrastructureDependencyInjection(application);
        }

        private void AdjustInfrastructureDependencyInjection(IApplication application)
        {

            var diTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

            diTemplate?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var addMassTransitConfigurationMethod = @class.FindMethod("AddInfrastructure");

                addMassTransitConfigurationMethod.AddParameter($"IEnumerable<{diTemplate.GetModuleInstallerInterfaceName()}>", "moduleInstallers");
                var statement = addMassTransitConfigurationMethod.FindStatement(m => m.Text.StartsWith("services.AddMassTransitConfiguration"));
                statement.Replace(statement.GetText("").Replace(")", ", moduleInstallers)"));
            }, 1);

        }

        private void RegisterStartup(IApplication application)
        {
            var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            startup?.CSharpFile.AfterBuild(file =>
            {
                startup.StartupFile.ConfigureServices((statements, context) =>
                {
                    startup.GetTypeName(application.FindTemplateInstance<ICSharpFileBuilderTemplate>(ModuleInstallerInterfaceTemplate.TemplateId));
                    var statement = statements.FindStatement(m => m.Text.StartsWith($"{context.Services}.AddApplication"));

                    if (statement is null) return;

                    statement.InsertAbove($"var moduleInstallers = ModuleInstallerFactory.GetModuleInstallers();");
                    statement.InsertBelow(new CSharpStatement($"moduleInstallers.ConfigureContainer({context.Services}, {context.Configuration});"));

                    statement = statements.FindStatement(m => m.Text.StartsWith($"{context.Services}.AddInfrastructure"));
                    if (statement is not null)
                    {
                        statement.Replace(statement.GetText("").Replace(")", " ,moduleInstallers)"));
                    }
                    statement = statements.FindStatement(m => m.Text.StartsWith($"{context.Services}.ConfigureSwagger"));
                    if (statement is not null)
                    {
                        statement.Replace(statement.GetText("").Replace(")", " ,moduleInstallers)"));
                    }

                    statement = statements.FindStatement(m => m.Text.StartsWith($"{context.Services}.AddMassTransitConfiguration"));
                    if (statement is not null)
                    {
                        statement.Replace(statement.GetText("").Replace(")", " ,moduleInstallers)"));
                    }
                });
            });
        }
    }
}