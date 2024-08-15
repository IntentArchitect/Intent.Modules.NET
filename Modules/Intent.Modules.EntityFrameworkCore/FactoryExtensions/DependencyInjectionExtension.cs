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
using Intent.Modules.EntityFrameworkCore.Helpers;
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

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dependencyInjectionTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (dependencyInjectionTemplate is null)
            {
                return;
            }

            var dbContexts = DbContextManager.GetDbContexts(application.Id, application.MetadataManager);
            ApplyApplicationSettings(dependencyInjectionTemplate, application, dbContexts);
            ApplyConfigurationStatements(dependencyInjectionTemplate, dbContexts);
        }

        private static void ApplyApplicationSettings(ICSharpFileBuilderTemplate dependencyInjectionTemplate, IApplication application, IEnumerable<DbContextInstance> dbContexts)
        {
            foreach (var dbContextInstance in dbContexts)
            {
                var targetDbProvider = DbContextManager.GetDatabaseProviderForDbContext(dbContextInstance.DbProvider, dependencyInjectionTemplate.ExecutionContext);
                switch (targetDbProvider)
                {
                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreInMemory(dependencyInjectionTemplate.OutputTarget.GetProject()));
                        break;

                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreSqlServer(dependencyInjectionTemplate.OutputTarget.GetProject()));

                        if (ShouldInstallErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(dependencyInjectionTemplate.OutputTarget))
                        {
                            dependencyInjectionTemplate.AddNugetDependency(
                                NugetPackages.ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(dependencyInjectionTemplate.OutputTarget));
                        }
                        else
                        {
                            application.EventDispatcher.Publish(new RemoveNugetPackageEvent(
                                NugetPackages.ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(dependencyInjectionTemplate.OutputTarget).Name,
                                dependencyInjectionTemplate.OutputTarget));
                        }

                        application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                            name: dbContextInstance.ConnectionStringName,
                            connectionString:
                            $"Server=.;Initial Catalog={dependencyInjectionTemplate.OutputTarget.ApplicationName()};Integrated Security=true;MultipleActiveResultSets=True{GetSqlServerExtendedConnectionString(dependencyInjectionTemplate.OutputTarget.GetProject())}",
                            providerName: "System.Data.SqlClient"));
                        application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.SqlServer.Name)
                            .WithProperty(Infrastructure.SqlServer.Property.ConnectionStringName, dbContextInstance.ConnectionStringName));
                        break;

                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(dependencyInjectionTemplate.OutputTarget.GetProject()));
                        application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                            name: dbContextInstance.ConnectionStringName,
                            connectionString:
                            $"Host=127.0.0.1;Port=5432;Database={dependencyInjectionTemplate.OutputTarget.ApplicationName()};Username=postgres;Password=password;",
                            providerName: ""));
                        application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.PostgreSql.Name)
                            .WithProperty(Infrastructure.PostgreSql.Property.ConnectionStringName, dbContextInstance.ConnectionStringName));
                        break;

                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.PomeloEntityFrameworkCoreMySql(dependencyInjectionTemplate.OutputTarget.GetProject()));
                        application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                            name: dbContextInstance.ConnectionStringName,
                            connectionString: $"Server=localhost;Database={dependencyInjectionTemplate.OutputTarget.ApplicationName()};Uid=root;Pwd=P@ssw0rd;",
                            providerName: ""));
                        application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.MySql.Name)
                            .WithProperty(Infrastructure.MySql.Property.ConnectionStringName, dbContextInstance.ConnectionStringName));
                        break;

                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreCosmos(dependencyInjectionTemplate.OutputTarget.GetProject()));
                        application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountEndpoint", "https://localhost:8081"));
                        application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:AccountKey",
                            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
                        application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:DatabaseName",
                            $"{dependencyInjectionTemplate.OutputTarget.GetProject().ApplicationName()}DB"));
                        application.EventDispatcher.Publish(new AppSettingRegistrationRequest($"{ConfigSectionCosmos}:EnsureDbCreated", true));
                        application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.CosmosDb.Name)
                            .WithProperty(Infrastructure.CosmosDb.Property.ConnectionStringName, dbContextInstance.ConnectionStringName));
                        break;

                    case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Oracle:
                        dependencyInjectionTemplate.AddNugetDependency(NugetPackages.OracleEntityFrameworkCore(dependencyInjectionTemplate.OutputTarget.GetProject()));
                        application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                            name: dbContextInstance.ConnectionStringName,
                            connectionString:
                            $"Data Source={dependencyInjectionTemplate.OutputTarget.ApplicationName()};User Id=myUsername;Password=myPassword;Integrated Security=no;",
                            providerName: ""));
                        application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Oracle.Name)
                            .WithProperty(Infrastructure.Oracle.Property.ConnectionStringName, dbContextInstance.ConnectionStringName));
                        break;

                    default:
                        break;
                }
            }
        }

        private static string GetSqlServerExtendedConnectionString(ICSharpProject project)
        {
            return project.TryGetMaxNetAppVersion(out var version) && version.Major >= 7
                ? ";Encrypt=False"
                : string.Empty;
        }

        private static void ApplyConfigurationStatements(ICSharpFileBuilderTemplate dependencyInjectionTemplate, IEnumerable<DbContextInstance> dbContexts)
        {
            dependencyInjectionTemplate.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.EntityFrameworkCore");
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                foreach (var dbContextInstance in dbContexts)
                {
                    method.AddStatement(CreateAddDbContextStatement(dependencyInjectionTemplate, dbContextInstance, dependencyInjectionTemplate.ExecutionContext));
                }
            });
        }

        private static AddDbContextStatement CreateAddDbContextStatement(ICSharpFileBuilderTemplate dependencyInjection,
            DbContextInstance dbContextInstance,
            ISoftwareFactoryExecutionContext executionContext)
        {
            var connectionString = $@"""{dbContextInstance.ConnectionStringName}""";

            var addDbContext = new AddDbContextStatement(dbContextInstance.GetTypeName(dependencyInjection));
            var statements = new List<CSharpStatement>();

            var enableSplitQueriesGlobally = dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().EnableSplitQueriesGlobally();

            var migrationsAssemblyStatement = $"MigrationsAssembly(typeof({dbContextInstance.DbContextName}).Assembly.FullName)";

            var dbContextOptionsBuilderStatement = new CSharpLambdaBlock("b");
            var builderStatements = new List<CSharpStatement>();
            builderStatements.Add($"b.{migrationsAssemblyStatement}");
            if (enableSplitQueriesGlobally)
            {
                builderStatements.Add("b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)");
            }

            if (dbContextInstance.IsApplicationDbContext && !string.IsNullOrWhiteSpace(dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().DefaultSchemaName()))
            {
                dependencyInjection.AddUsing("Microsoft.EntityFrameworkCore.Migrations");
                builderStatements.Add($@"b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, ""{dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().DefaultSchemaName()}"");");
            }

            var targetDbProvider = DbContextManager.GetDatabaseProviderForDbContext(dbContextInstance.DbProvider, executionContext);
            switch (targetDbProvider)
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                    dependencyInjection.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreInMemory(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseInMemoryDatabase")
                        .AddArgument($"{connectionString}", a => a.AddMetadata("is-connection-string", true)));
                    break;

                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    dependencyInjection.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreSqlServer(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseSqlServer")
                        .WithArgumentsOnNewLines()
                        .AddArgument($"configuration.GetConnectionString({connectionString})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument(dbContextOptionsBuilderStatement));

                    if (ShouldInstallErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(dependencyInjection.OutputTarget))
                    {
                        builderStatements.Add("b.UseDateOnlyTimeOnly()");
                    }


                    ConfigureNetTopologySuite(dependencyInjection, dbContextInstance, builderStatements, NugetPackages.MicrosoftEntityFrameworkCoreSqlServerNetTopologySuite(dependencyInjection.OutputTarget));

                    break;

                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    dependencyInjection.AddNugetDependency(NugetPackages.NpgsqlEntityFrameworkCorePostgreSQL(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseNpgsql")
                        .WithArgumentsOnNewLines()
                        .AddArgument($"configuration.GetConnectionString({connectionString})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument(dbContextOptionsBuilderStatement));

                    ConfigureNetTopologySuite(dependencyInjection, dbContextInstance, builderStatements, NugetPackages.NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuite(dependencyInjection.OutputTarget));

                    break;

                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                    dependencyInjection.AddNugetDependency(NugetPackages.PomeloEntityFrameworkCoreMySql(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseMySql")
                        .AddArgument($"configuration.GetConnectionString({connectionString})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument(@"ServerVersion.Parse(""8.0"")")
                        .AddArgument(dbContextOptionsBuilderStatement));

                    ConfigureNetTopologySuite(dependencyInjection, dbContextInstance, builderStatements, NugetPackages.PomeloEntityFrameworkCoreMySqlNetTopologySuite(dependencyInjection.OutputTarget));

                    break;

                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    dependencyInjection.AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreCosmos(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseCosmos")
                        .WithArgumentsOnNewLines()
                        .AddArgument(@"configuration[""Cosmos:AccountEndpoint""]")
                        .AddArgument(@"configuration[""Cosmos:AccountKey""]")
                        .AddArgument(@"configuration[""Cosmos:DatabaseName""]", a => a.AddMetadata("is-connection-string", true)));
                    break;

                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Oracle:
                    dependencyInjection.AddNugetDependency(NugetPackages.OracleEntityFrameworkCore(dependencyInjection.OutputTarget.GetProject()));

                    statements.Add(new CSharpInvocationStatement("options.UseOracle")
                        .WithArgumentsOnNewLines()
                        .AddArgument($"configuration.GetConnectionString({connectionString})", a => a.AddMetadata("is-connection-string", true))
                        .AddArgument(dbContextOptionsBuilderStatement));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(null, "Database Provider has not been set to a valid value. Please fix in the Database Settings.");
            }

            if (builderStatements.Count == 1)
            {
                dbContextOptionsBuilderStatement.WithExpressionBody(builderStatements.First());
            }
            else
            {
                foreach (var statement in builderStatements)
                {
                    dbContextOptionsBuilderStatement.AddStatement(statement.WithSemicolon());
                }
            }

            if (dependencyInjection.ExecutionContext.Settings.GetDatabaseSettings().LazyLoadingWithProxies())
            {
                statements.Add("options.UseLazyLoadingProxies();");
            }

            addDbContext.AddConfigOptionStatements(statements);
            return addDbContext;
        }

        private static void ConfigureNetTopologySuite(ICSharpFileBuilderTemplate dependencyInjection, DbContextInstance dbContextInstance, List<CSharpStatement> builderStatements,
            NugetPackageInfo nuget)
        {
            // Determining which DbContext is using the Geometry types and targeting that one only requires that you determine which
            // Domain package contains Models that are referencing those types. A simple way to achieve that is to have the TypeResolver
            // publish an Event (which needs to be introduced to both EF and NetTopologySuite modules) and upon detection figure out
            // the package in question. Not doing that for now and only assuming main DB context is using this.
            if (NetTopologySuiteHelper.IsInstalled(dependencyInjection.ExecutionContext) && dbContextInstance.IsApplicationDbContext)
            {
                dependencyInjection.AddNugetDependency(nuget);
                builderStatements.Add("b.UseNetTopologySuite();");
            }
        }

        public static bool ShouldInstallErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(IOutputTarget outputTarget)
        {
            return outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => true,
                (7, 0) => true,
                _ => false
            };
        }

    }
}