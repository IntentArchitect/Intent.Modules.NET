using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DependencyInjectionExtension";

        private const string ConfigSectionCosmos = "Cosmos";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            switch (dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(dependencyInjection.OutputTarget.GetProject()));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(dependencyInjection.OutputTarget.GetProject()));
                    application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                        name: "DefaultConnection",
                        connectionString: $"Server=.;Initial Catalog={dependencyInjection.OutputTarget.ApplicationName()};Integrated Security=true;MultipleActiveResultSets=True{GetSqlServerExtendedConnectionString(dependencyInjection.OutputTarget.GetProject())}",
                        providerName: "System.Data.SqlClient"));
                    application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.SqlServer.Name)
                        .WithProperty(Infrastructure.SqlServer.Property.ConnectionStringName, "DefaultConnection"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    dependencyInjection.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(dependencyInjection.OutputTarget.GetProject()));
                    application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                        name: "DefaultConnection",
                        connectionString: $"Host=127.0.0.1;Port=5432;Database={dependencyInjection.OutputTarget.ApplicationName()};Username=postgres;Password=password;",
                        providerName: ""));
                    application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.PostgreSql.Name)
                        .WithProperty(Infrastructure.PostgreSql.Property.ConnectionStringName, "DefaultConnection"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                    dependencyInjection.AddNugetDependency(NugetPackages.MySqlEntityFrameworkCore(dependencyInjection.OutputTarget.GetProject()));
                    application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                        name: "DefaultConnection",
                        connectionString: $"Server=localhost;Database={dependencyInjection.OutputTarget.ApplicationName()};Uid=root;Pwd=P@ssw0rd;",
                        providerName: ""));
                    application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.MySql.Name)
                        .WithProperty(Infrastructure.MySql.Property.ConnectionStringName, "DefaultConnection"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(dependencyInjection.OutputTarget.GetProject()));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountEndpoint", "https://localhost:8081"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DatabaseName", $"{dependencyInjection.OutputTarget.GetProject().ApplicationName()}DB"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:EnsureDbCreated", true));
                    application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.CosmosDb.Name)
                        .WithProperty(Infrastructure.CosmosDb.Property.ConnectionStringName, "DefaultConnection"));
                    break;
                default:
                    break;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.EntityFrameworkCore");
                file.Classes.First().FindMethod("AddInfrastructure")
                    .AddStatement(CreateAddDbContextStatement(dependencyInjection));
            });
        }

        private string GetSqlServerExtendedConnectionString(ICSharpProject project)
        {
            if (project.IsNetApp(7))
            {
                return ";Encrypt=False";
            }
            return string.Empty;
        }

        private AddDbContextStatement CreateAddDbContextStatement(ICSharpFileBuilderTemplate dependencyInjection)
        {
            var addDbContext = new AddDbContextStatement(dependencyInjection.GetDbContextName());
            var connection = "\"DefaultConnection\"";
            var statements = new List<CSharpStatement>();

            switch (dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement($@"options.UseInMemoryDatabase")
                        .AddArgument($@"{connection}", a => a.AddMetadata("is-connection-string", true)));
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement($@"options.UseSqlServer")
                        .WithArgumentsOnNewLines()
                        .AddArgument($@"configuration.GetConnectionString({connection})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument($@"b => b.MigrationsAssembly(typeof({dependencyInjection.GetDbContextName()}).Assembly.FullName)"));
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    dependencyInjection.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement($@"options.UseNpgsql")
                        .WithArgumentsOnNewLines()
                        .AddArgument($@"configuration.GetConnectionString({connection})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument($@"b => b.MigrationsAssembly(typeof({dependencyInjection.GetDbContextName()}).Assembly.FullName)"));
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                    dependencyInjection.AddNugetDependency(NugetPackages.MySqlEntityFrameworkCore(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement($@"options.UseMySql")
                        .AddArgument($"configuration.GetConnectionString({connection})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument($@"ServerVersion.Parse(""8.0"")"));
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    dependencyInjection.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement($@"options.UseCosmos")
                        .WithArgumentsOnNewLines()
                        .AddArgument(@"configuration[""Cosmos:AccountEndpoint""]")
                        .AddArgument(@"configuration[""Cosmos:AccountKey""]")
                        .AddArgument(@"configuration[""Cosmos:DatabaseName""]", a => a.AddMetadata("is-connection-string", true)));
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, "Database Provider has not been set to a valid value. Please fix in the Database Settings.");
            }

            addDbContext.AddConfigOptionStatements(statements);
            return addDbContext;
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.EntityFrameworkCore";
        }
    }
}