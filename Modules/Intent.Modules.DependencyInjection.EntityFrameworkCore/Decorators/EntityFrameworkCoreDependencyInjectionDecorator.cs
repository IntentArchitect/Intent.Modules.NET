using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Settings;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDependencyInjectionDecorator : DependencyInjectionDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.DependencyInjection.EntityFrameworkCore.EntityFrameworkCoreDependencyInjectionDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DependencyInjectionTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        private const string ConfigSectionSqlServer = "SqlServer";
        private const string ConfigSectionPostgreSql = "PostgreSql";
        private const string ConfigSectionCosmos = "Cosmos";
        
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public EntityFrameworkCoreDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            if (_template.Project.IsNet5App())
            {
                _template.AddNugetDependency("Microsoft.Extensions.Configuration.Binder", "5.0.0");
            }
            if (_template.Project.IsNet6App())
            {
                _template.AddNugetDependency("Microsoft.Extensions.Configuration.Binder", "6.0.0");
            }

            // TODO: GCB - These need to be registered here, otherwise it doesn't work. Look to understand why...
            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(_template.Project));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(_template.Project));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionSqlServer}:DefaultSchemaName", ""));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    _template.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(_template.Project));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionPostgreSql}:DefaultSchemaName", ""));
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(_template.Project));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountEndpoint", "https://localhost:8081"));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountKey", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DatabaseName", $"{_template.Project.ApplicationName()}DB"));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DefaultContainerName", $"{_template.Project.ApplicationName()}"));
                    _application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:PartitionKey", $""));
                    break;
                default:
                    break;
            }

            if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            {
                _template.AddNugetDependency("Finbuckle.MultiTenant", "6.5.1");
                _template.AddNugetDependency("Finbuckle.MultiTenant.EntityFrameworkCore", "6.5.1");
            }

            _template.ExecutionContext.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                name: "DefaultConnection",
                connectionString: $"Server=.;Initial Catalog={_template.OutputTarget.ApplicationName()};Integrated Security=true;MultipleActiveResultSets=True",
                providerName: "System.Data.SqlClient"));
        }

        public override string ServiceRegistration()
        {
            var statements = new List<string>();
            statements.Add($@"services.AddDbContext<{_template.GetDbContextName()}>((sp, options) =>
            {{
                {GetDbContextOptions()}
            }});");

            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionSqlServer}"").Bind(opt));");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionPostgreSql}"").Bind(opt));");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    statements.Add($@"services.Configure<{_template.GetDbContextConfigurationName()}>(opt => configuration.GetSection(""{ConfigSectionCosmos}"").Bind(opt));");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                default:
                    break;
            }
            
            return string.Join(@"
            ", statements);
        }

        private string GetDbContextOptions()
        {
            var connection = "\"DefaultConnection\"";
            var statements = new List<string>();
            if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            {
                statements.Add($@"var tenantInfo = sp.GetService<{_template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {_template.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");");
                connection = "tenantInfo.ConnectionString";
            }

            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(_template.Project));
                    statements.Add($@"options.UseInMemoryDatabase({connection});");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(_template.Project));
                    statements.Add($@"options.UseSqlServer(
                    configuration.GetConnectionString({connection}),
                    b => b.MigrationsAssembly(typeof({_template.GetDbContextName()}).Assembly.FullName));");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    _template.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(_template.Project));
                    statements.Add($@"options.UseNpgsql(
                    configuration.GetConnectionString({connection}),
                    b => b.MigrationsAssembly(typeof({_template.GetDbContextName()}).Assembly.FullName));");
                    statements.Add($@"options.UseLazyLoadingProxies();");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreCosmos(_template.Project));
                    statements.Add($@"options.UseCosmos(
                    configuration[""Cosmos:AccountEndpoint""],
                    configuration[""Cosmos:AccountKey""],
                    configuration[""Cosmos:DatabaseName""]);");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, "Database Provider has not been set to a valid value. Please fix in the Database Settings.");
            }

            return string.Join(@"
                    ", statements);
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.EntityFrameworkCore";
        }
    }
}