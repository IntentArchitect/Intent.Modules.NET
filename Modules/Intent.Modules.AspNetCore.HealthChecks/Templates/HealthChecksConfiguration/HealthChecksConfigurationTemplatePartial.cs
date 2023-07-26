using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
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
                        method.AddStatement("var hcBuilder = services.AddHealthChecks();", stmt => stmt.AddMetadata("health-checks-builder", true));
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
                });
            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(Handle);
        }

        private void Handle(InfrastructureRegisteredEvent @event)
        {
            CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("ConfigureHealthChecks");
                var hcBuilder = method.FindStatement(p => p.HasMetadata("health-checks-builder"));
                hcBuilder.InsertBelow(GetKnownHealthConfigurationStatement(@event));
            }, 10);
        }

        private CSharpStatement GetKnownHealthConfigurationStatement(InfrastructureRegisteredEvent @event)
        {
            var registration = GetHealthCheckRegistrationDetail(@event);
            var configStmt = new CSharpInvocationStatement(registration.ExtensionMethod);

            switch (registration.Type)
            {
                case InfrastructureType.Database:
                    if (@event.ConnectionDetails.TryGetValue(InfrastructureComponent.ConnectionDetail.ConnectionStringName, out var connectionStringName))
                    {
                        configStmt.AddArgument($@"configuration.GetConnectionString(""{connectionStringName}"")");
                    }
                    else if (@event.ConnectionDetails.TryGetValue(InfrastructureComponent.ConnectionDetail.ConnectionStringSettingPath, out var connectionStringSettingPath))
                    {
                        configStmt.AddArgument($@"configuration[""{connectionStringSettingPath}""]");
                    }
                    else
                    {
                        throw new Exception($"Registered Infrastructure Event {@event.InfrastructureComponent} did not specify connection detail of {nameof(InfrastructureComponent.ConnectionDetail.ConnectionStringName)} or {nameof(InfrastructureComponent.ConnectionDetail.ConnectionStringSettingPath)}");
                    }
                    
                    configStmt.AddArgument($@"name: ""{GetConnectionDetail(@event, InfrastructureComponent.ConnectionDetail.DatabaseName)}""");
                    break;
                default:
                    break;
            }

            configStmt.AddArgument(@"tags: new [] { ""ready"" }");

            AddNugetDependency(registration.NugetPackage);
            return configStmt;
        }
        
        private static string GetConnectionDetail(InfrastructureRegisteredEvent @event, string key)
        {
            if (@event.ConnectionDetails.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new Exception($"Registered Infrastructure Event {@event.InfrastructureComponent} did not specify connection detail of {key}");
        }

        private enum InfrastructureType
        {
            Database
        }

        private record HealthCheckRegistration(string ExtensionMethod, INugetPackageInfo NugetPackage, InfrastructureType Type);

        private HealthCheckRegistration GetHealthCheckRegistrationDetail(InfrastructureRegisteredEvent @event)
        {
            return @event.InfrastructureComponent switch
            {
                InfrastructureComponent.SqlServer => new HealthCheckRegistration("hcBuilder.AddSqlServer", NugetPackage.AspNetCoreHealthChecksSqlServer(OutputTarget),
                    InfrastructureType.Database),
                InfrastructureComponent.PostgreSql => new HealthCheckRegistration("hcBuilder.AddNpgSql", NugetPackage.AspNetCoreHealthChecksNpgSql(OutputTarget),
                    InfrastructureType.Database),
                InfrastructureComponent.MySql => new HealthCheckRegistration("hcBuilder.AddMySql", NugetPackage.AspNetCoreHealthChecksMySql(OutputTarget),
                    InfrastructureType.Database),
                InfrastructureComponent.CosmosDb => new HealthCheckRegistration("hcBuilder.AddCosmosDb", NugetPackage.AspNetCoreHealthChecksCosmosDb(OutputTarget),
                    InfrastructureType.Database),
                _ => null
            };
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureHealthChecks", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            var startup = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("App.Startup"));
            startup?.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                ((IHasCSharpStatements)priClass.FindMethod("Configure")
                        ?.FindStatement(s => s.GetText("").Contains("UseEndpoints")))
                    ?.InsertStatement(0, $"endpoints.MapDefaultHealthChecks();");
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