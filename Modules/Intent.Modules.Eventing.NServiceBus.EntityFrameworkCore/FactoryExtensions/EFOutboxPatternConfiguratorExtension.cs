using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using NugetPackages = Intent.Modules.Eventing.NServiceBus.NugetPackages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EFOutboxPatternConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.NServiceBus.EntityFrameworkCore.EFOutboxPatternConfiguratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetNServiceBusSettings().OutboxPattern().IsEntityFramework())
            {
                return;
            }

            var configTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate(NServiceBusConfigurationTemplate.TemplateId));

            if (configTemplate == null)
            {
                Logging.Log.Warning("NServiceBus EF outbox: NServiceBusConfiguration template not found. Outbox configuration will not be applied.");
                return;
            }

            var dbContextTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate("Intent.EntityFrameworkCore.DbContext"));

            if (dbContextTemplate == null)
            {
                Logging.Log.Warning("NServiceBus EF outbox: DbContext template not found. Ensure Intent.EntityFrameworkCore is installed.");
                return;
            }

            configTemplate.AddNugetDependency(NugetPackages.NServiceBusPersistenceSql(configTemplate.OutputTarget));

            configTemplate.CSharpFile.OnBuild(file =>
            {
                var configClass = file.Classes.First();
                var configureEndpoint = configClass.FindMethod("ConfigureEndpoint");
                if (configureEndpoint == null)
                {
                    return;
                }

                // Find the endpointConfiguration.EnableInstallers() statement to insert persistence before it
                var enableInstallersStmt = configureEndpoint.FindStatement(s => s.ToString()!.Contains("EnableInstallers"));

                var dbContextName = dbContextTemplate.GetTypeName("Intent.EntityFrameworkCore.DbContext");
                var connectionStringStmt = configureEndpoint.FindStatement(s =>
                    s.ToString()!.Contains("GetConnectionString") || s.ToString()!.Contains("connectionString"));

                var persistenceStatements = new[]
                {
                    $"var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();",
                    $"persistence.ConnectionBuilder(cancellationToken => new Microsoft.Data.SqlClient.SqlConnection(configuration.GetConnectionString(\"DefaultConnection\") ?? throw new InvalidOperationException(\"ConnectionStrings:DefaultConnection is not configured\")));",
                    $"persistence.SqlDialect<SqlDialect.MsSqlServer>();",
                    $"endpointConfiguration.EnableOutbox();"
                };

                if (enableInstallersStmt != null)
                {
                    foreach (var stmt in persistenceStatements.Reverse())
                    {
                        enableInstallersStmt.InsertAbove(new CSharpStatement(stmt));
                    }
                    enableInstallersStmt.SeparatedFromPrevious();
                }
                else
                {
                    foreach (var stmt in persistenceStatements)
                    {
                        configureEndpoint.AddStatement(new CSharpStatement(stmt));
                    }
                }

                file.AddUsing("NServiceBus.Persistence.Sql");
            }, 10);
        }
    }
}
