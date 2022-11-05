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

        private const string ConfigSectionSqlServer = "SqlServer";
        private const string ConfigSectionPostgreSql = "PostgreSql";
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
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.DependencyInjection");
            if (template == null)
            {
                return;
            }
            if (template.OutputTarget.GetProject().IsNet5App() || template.OutputTarget.GetProject().IsNet6App())
            {
                template.AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(template.OutputTarget.GetProject()));
            }

            switch (template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(template.OutputTarget.GetProject()));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(template.OutputTarget.GetProject()));
                    //application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionSqlServer}:DefaultSchemaName", ""));
                    //application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionSqlServer}:EnsureDbCreated", false));
                    application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                        name: "DefaultConnection",
                        connectionString: $"Server=.;Initial Catalog={template.OutputTarget.ApplicationName()};Integrated Security=true;MultipleActiveResultSets=True",
                        providerName: "System.Data.SqlClient"));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    template.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(template.OutputTarget.GetProject()));
                    //application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionPostgreSql}:DefaultSchemaName", ""));
                    //application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionPostgreSql}:EnsureDbCreated", false));
                    application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                        name: "DefaultConnection",
                        connectionString: $"Server=127.0.0.1;Port=5432;Database={template.OutputTarget.ApplicationName()};Integrated Security=true;",
                        providerName: ""));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    template.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(template.OutputTarget.GetProject()));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountEndpoint", "https://localhost:8081"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DatabaseName", $"{template.OutputTarget.GetProject().ApplicationName()}DB"));
                    //application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DefaultContainerName", $"{template.OutputTarget.GetProject().ApplicationName()}"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:EnsureDbCreated", true));
                    break;
                default:
                    break;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.EntityFrameworkCore");
                file.Classes.First().FindMethod("AddInfrastructure")
                    .AddStatement(new AddDbContextStatement(template.GetDbContextName())
                        .AddConfigOptionStatements(GetDbContextOptions(template)));
            });


            //if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            //{
            //    _template.AddNugetDependency("Finbuckle.MultiTenant", "6.5.1");
            //    _template.AddNugetDependency("Finbuckle.MultiTenant.EntityFrameworkCore", "6.5.1");
            //}
        }





        //public string ServiceRegistration(ICSharpFileBuilderTemplate _template)
        //{
        //    var statements = new List<string>();
        //    statements.Add($@"services.AddDbContext<{_template.GetDbContextName()}>((sp, options) =>
        //    {{
        //        {GetDbContextOptions(_template)}
        //    }});");

        //    switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
        //    {
        //        case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
        //            statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionSqlServer}"").Bind(opt));");
        //            break;
        //        case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
        //            statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionPostgreSql}"").Bind(opt));");
        //            break;
        //        case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
        //            statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionCosmos}"").Bind(opt));");
        //            break;
        //        case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
        //        default:
        //            break;
        //    }

        //    return string.Join(@"
        //    ", statements);
        //}

        private IEnumerable<CSharpStatement> GetDbContextOptions(ICSharpFileBuilderTemplate _template)
        {
            var connection = "\"DefaultConnection\"";
            var statements = new List<CSharpStatement>();
            //if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            //{
            //    statements.Add($@"var tenantInfo = sp.GetService<{_template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {_template.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");");
            //    connection = "tenantInfo.ConnectionString";
            //}

            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(_template.OutputTarget.GetProject()));
                    statements.Add($@"options.UseInMemoryDatabase({connection});");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(_template.OutputTarget.GetProject()));
                    statements.Add($@"options.UseSqlServer(
                    configuration.GetConnectionString({connection}),
                    b => b.MigrationsAssembly(typeof({_template.GetDbContextName()}).Assembly.FullName));");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    _template.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(_template.OutputTarget.GetProject()));
                    statements.Add($@"options.UseNpgsql(
                    configuration.GetConnectionString({connection}),
                    b => b.MigrationsAssembly(typeof({_template.GetDbContextName()}).Assembly.FullName));");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(_template.OutputTarget.GetProject()));
                    statements.Add($@"options.UseCosmos(
                    configuration[""Cosmos:AccountEndpoint""],
                    configuration[""Cosmos:AccountKey""],
                    configuration[""Cosmos:DatabaseName""]);");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, "Database Provider has not been set to a valid value. Please fix in the Database Settings.");
            }

            return statements;
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.EntityFrameworkCore";
        }
    }
}