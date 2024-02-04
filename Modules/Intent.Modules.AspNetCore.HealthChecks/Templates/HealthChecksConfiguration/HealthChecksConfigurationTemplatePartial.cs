using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.HealthChecks.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.HealthChecks.Templates.HealthChecksConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HealthChecksConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.HealthChecks.HealthChecksConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HealthChecksConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackage.AspNetCoreHealthChecksUIClient(outputTarget));

            if (ExecutionContext.Settings.GetHealthChecks().PublishEvents().IsAzureApplicationInsights())
            {
                AddNugetDependency(NugetPackage.AspNetcoreHealthChecksPublisherApplicationInsights(outputTarget));
            }

            if (ExecutionContext.Settings.GetHealthChecks().HealthChecksUI())
            {
                AddNugetDependency(NugetPackage.AspNetCoreHealthChecksUI(outputTarget));
                AddNugetDependency(NugetPackage.AspNetCoreHealthChecksUIInMemoryStorage(outputTarget));
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("HealthChecks.UI.Client")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Diagnostics.HealthChecks")
                .AddUsing("Microsoft.AspNetCore.Routing")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"HealthChecksConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureHealthChecks", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("var hcBuilder = services.AddHealthChecks();",
                            stmt => stmt.AddMetadata("health-checks-builder", true));

                        AddEventPublishing(method);
                        AddHealthCheckUi(method);

                        method.AddStatement("return services;", stmt => stmt.SeparatedFromPrevious());
                    });

                    @class.AddMethod("IEndpointRouteBuilder", "MapDefaultHealthChecks", method =>
                    {
                        method.Static();
                        method.AddParameter("IEndpointRouteBuilder", "endpoints", param => param.WithThisModifier());
                        method.AddInvocationStatement("endpoints.MapHealthChecks", mapHealthCheck =>
                        {
                            mapHealthCheck.AddArgument(@"""/hc""");
                            mapHealthCheck.AddArgument(new CSharpObjectInitializerBlock("new HealthCheckOptions")
                                .AddInitStatement("Predicate", "_ => true")
                                .AddInitStatement("ResponseWriter", "UIResponseWriter.WriteHealthCheckUIResponse"));
                        });
                        method.AddStatement("return endpoints;");
                    });

                    if (ExecutionContext.Settings.GetHealthChecks().HealthChecksUI())
                    {
                        @class.AddMethod("IEndpointRouteBuilder", "MapDefaultHealthChecksUI", method =>
                        {
                            method.Static();
                            method.AddParameter("IEndpointRouteBuilder", "endpoints", param => param.WithThisModifier());
                            method.AddInvocationStatement("endpoints.MapHealthChecksUI", mapHealthCheckUi =>
                            {
                                mapHealthCheckUi.AddArgument(new CSharpLambdaBlock("settings")
                                    .WithExpressionBody(@"settings.UIPath = ""/hc-ui"""));
                            });
                            method.AddStatement("return endpoints;");
                        });
                    }
                });
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void AddEventPublishing(CSharpClassMethod method)
        {
            switch (ExecutionContext.Settings.GetHealthChecks().PublishEvents().AsEnum())
            {
                case Settings.HealthChecks.PublishEventsOptionsEnum.None:
                    break;
                case Settings.HealthChecks.PublishEventsOptionsEnum.AzureApplicationInsights:
                    this.ApplyAppSetting("ApplicationInsights:ConnectionString",
                        "Insert Application Insights Connection String Here");
                    method.AddStatement($@"hcBuilder.AddApplicationInsightsPublisher(connectionString: configuration[""ApplicationInsights:ConnectionString""]);");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddHealthCheckUi(CSharpClassMethod method)
        {
            if (!ExecutionContext.Settings.GetHealthChecks().HealthChecksUI()) { return; }

            AddNugetDependency(NugetPackage.AspNetCoreHealthChecksUI(OutputTarget));
            AddNugetDependency(NugetPackage.AspNetCoreHealthChecksUIInMemoryStorage(OutputTarget));

            method.AddInvocationStatement("services.AddHealthChecksUI", stmt => stmt
                .AddArgument(new CSharpLambdaBlock("settings")
                    .AddStatement(@$"settings.AddHealthCheckEndpoint(""{ExecutionContext.GetApplicationConfig().Name}"", ""/hc"");"))
                .WithoutSemicolon());
            method.AddStatement(".AddInMemoryStorage();");
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("ConfigureHealthChecks");
                var hcBuilder = method.FindStatement(p => p.HasMetadata("health-checks-builder"));
                var healthCheckConfigStatements = GetKnownHealthConfigurationStatements(@event);
                hcBuilder.InsertBelow(healthCheckConfigStatements.ToArray());
            }, 10);
        }

        private IEnumerable<CSharpStatement> GetKnownHealthConfigurationStatements(InfrastructureRegisteredEvent @event)
        {
            switch (@event.InfrastructureComponent)
            {
                case Infrastructure.SqlServer.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksSqlServer(OutputTarget));
                    return GetDatabaseHealthCheckStatements(
                        @event: @event,
                        expression: "hcBuilder.AddSqlServer",
                        connectionStringNameVar: Infrastructure.SqlServer.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.SqlServer.Property.ConnectionStringSettingPath);
                case Infrastructure.PostgreSql.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksNpgSql(OutputTarget));
                    return GetDatabaseHealthCheckStatements(
                        @event: @event,
                        expression: "hcBuilder.AddNpgSql",
                        connectionStringNameVar: Infrastructure.PostgreSql.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.PostgreSql.Property.ConnectionStringSettingPath);
                case Infrastructure.MySql.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksMySql(OutputTarget));
                    return GetDatabaseHealthCheckStatements(
                        @event: @event,
                        expression: "hcBuilder.AddMySql",
                        connectionStringNameVar: Infrastructure.MySql.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.MySql.Property.ConnectionStringSettingPath);
                case Infrastructure.CosmosDb.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksCosmosDb(OutputTarget));
                    return GetCosmosDbStatements(
                        @event: @event,
                        connectionStringNameVar: Infrastructure.CosmosDb.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.CosmosDb.Property.ConnectionStringSettingPath);
                case Infrastructure.MongoDb.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksMongoDb(OutputTarget));
                    return GetDatabaseHealthCheckStatements(
                        @event: @event,
                        expression: "hcBuilder.AddMongoDb",
                        connectionStringNameVar: Infrastructure.MongoDb.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.MongoDb.Property.ConnectionStringSettingPath);
                case Infrastructure.Oracle.Name:
                    AddNugetDependency(NugetPackage.AspNetCoreHealthChecksOracle(OutputTarget));
                    return GetDatabaseHealthCheckStatements(
                        @event: @event,
                        expression: "hcBuilder.AddOracle",
                        connectionStringNameVar: Infrastructure.Oracle.Property.ConnectionStringName,
                        connectionStringSettingPathVar: Infrastructure.Oracle.Property.ConnectionStringSettingPath);
                default:
                    return Enumerable.Empty<CSharpInvocationStatement>();
            }
        }

        private IEnumerable<CSharpInvocationStatement> GetCosmosDbStatements(
            InfrastructureRegisteredEvent @event,
            string connectionStringNameVar,
            string connectionStringSettingPathVar)
        {
            if (OutputTarget.GetMaxNetAppVersion().Major < 7)
            {
                return GetDatabaseHealthCheckStatements(
                    @event: @event,
                    expression: "hcBuilder.AddCosmosDb",
                    connectionStringNameVar: connectionStringNameVar,
                    connectionStringSettingPathVar: connectionStringNameVar);
            }

            var addSingletonStatement = new CSharpInvocationStatement("hcBuilder.Services.AddSingleton");
            addSingletonStatement.AddArgument($"_ => new {UseType("Microsoft.Azure.Cosmos.CosmosClient")}({GetConnectionStringArgument(@event, connectionStringNameVar, connectionStringSettingPathVar)})");

            var addCosmosStatement = new CSharpInvocationStatement("hcBuilder.AddAzureCosmosDB");

            addCosmosStatement.AddArgument($@"healthCheckName: ""{@event.InfrastructureComponent}""");
            addCosmosStatement.AddArgument(@"tags: new[] { ""database"" }");

            return new[]
            {
                addSingletonStatement,
                addCosmosStatement
            };
        }

        private static IEnumerable<CSharpInvocationStatement> GetDatabaseHealthCheckStatements(
            InfrastructureRegisteredEvent @event,
            string expression,
            string connectionStringNameVar,
            string connectionStringSettingPathVar)
        {
            var configStmt = new CSharpInvocationStatement(expression);
            var arg = GetConnectionStringArgument(@event, connectionStringNameVar, connectionStringSettingPathVar);

            configStmt.AddArgument(arg);
            configStmt.AddArgument($@"name: ""{@event.InfrastructureComponent}""");
            configStmt.AddArgument($@"tags: new[] {{ ""database"" }}");

            yield return configStmt;
        }

        private static string GetConnectionStringArgument(
            InfrastructureRegisteredEvent @event,
            string connectionStringNameVar,
            string connectionStringSettingPathVar)
        {
            if (@event.Properties.TryGetValue(connectionStringNameVar, out var connectionStringName))
            {
                return $@"configuration.GetConnectionString(""{connectionStringName}"")!";
            }

            if (@event.Properties.TryGetValue(connectionStringSettingPathVar, out var connectionStringSettingPath))
            {
                return $@"configuration[""{connectionStringSettingPath}""]!";
            }

            throw new Exception(
                $"Registered Infrastructure Event {@event.InfrastructureComponent} did not specify connection detail of {connectionStringNameVar} or {connectionStringSettingPathVar}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureHealthChecks", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            var startup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(TemplateDependency.OnTemplate(IAppStartupTemplate.RoleName));
            startup?.CSharpFile.AfterBuild(_ =>
            {
                startup.StartupFile.ConfigureEndpoints((statements, context) =>
                {
                    statements.InsertStatement(0, $"{context.Endpoints}.MapDefaultHealthChecks();");

                    if (ExecutionContext.Settings.GetHealthChecks().HealthChecksUI())
                    {
                        statements.InsertStatement(1, $"{context.Endpoints}.MapDefaultHealthChecksUI();");
                    }
                });
            });
        }

        [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}