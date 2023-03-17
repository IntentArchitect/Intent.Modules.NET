using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.OpenTelemetry.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.OpenTelemetry.Templates.OpenTelemetryConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class OpenTelemetryConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.OpenTelemetry.OpenTelemetryConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public OpenTelemetryConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.OpenTelemetry);
        AddNugetDependency(NugetPackages.OpenTelemetryExtensionsHosting);
        AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationAspNetCore);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("OpenTelemetry.Resources")
            .AddUsing("OpenTelemetry.Trace")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddClass($"OpenTelemetryConfiguration")
            .OnBuild(file =>
            {
                var priClass = file.Classes.First();
                priClass.Static();

                priClass.AddMethod("IServiceCollection", "AddTelemetryConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");
                    method.AddMethodChainStatement("services.AddOpenTelemetry()", main => main
                        .AddChainStatement(new CSharpInvocationStatement("ConfigureResource")
                            .AddArgument(new CSharpLambdaBlock("res")
                                .AddMethodChainStatement("res", res => res
                                    .AddChainStatement($@"AddService(""{Project.ApplicationName()}"")")
                                    .AddChainStatement("AddTelemetrySdk()")
                                    .AddChainStatement("AddEnvironmentVariableDetector()")))
                            .WithArgumentsOnNewLines()
                            .WithoutSemicolon()
                            .AddMetadata("telemetry-resource", true))
                        .AddChainStatement(new CSharpInvocationStatement("WithTracing")
                            .AddArgument(new CSharpLambdaBlock("trace")
                                .AddMethodChainStatement("trace", trace => trace
                                    .WithoutSemicolon()
                                    .AddChainStatement("AddAspNetCoreInstrumentation()")))
                            .WithArgumentsOnNewLines()
                            .WithoutSemicolon()
                            .AddMetadata("telemetry-tracing", true))
                        .AddMetadata("telemetry-config", true));
                    method.AddStatement("return services;");
                });
            })
            .OnBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("AddTelemetryConfiguration");
                var telemConfigStmt = (CSharpMethodChainStatement)method.FindStatement(stmt => stmt.HasMetadata("telemetry-config"));
                var telemTranceStmt = (CSharpInvocationStatement)telemConfigStmt.FindStatement(stmt => stmt.HasMetadata("telemetry-tracing"));
                var traceChain = (CSharpMethodChainStatement)((CSharpLambdaBlock)telemTranceStmt.Statements.First()).Statements.First();

                if (ExecutionContext.Settings.GetOpenTelemetry().HTTPInstrumentation())
                {
                    AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationHttp);
                    traceChain.AddChainStatement("AddHttpClientInstrumentation()");
                }

                if (ExecutionContext.Settings.GetOpenTelemetry().SQLInstrumentation())
                {
                    AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationSqlClient);
                    traceChain.AddChainStatement("AddSqlClientInstrumentation()");
                }

                switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
                {
                    case Settings.OpenTelemetry.ExportOptionsEnum.ApplicationInsights:
                        AddNugetDependency(NugetPackages.AzureMonitorOpenTelemetryExporter);
                        file.AddUsing("Azure.Monitor.OpenTelemetry.Exporter");
                        traceChain.AddChainStatement(new CSharpInvocationStatement("AddAzureMonitorTraceExporter")
                            .AddStatement(new CSharpLambdaBlock("options")
                                .AddStatement(@"options.ConnectionString = configuration[""ApplicationInsights:ConnectionString""];")));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
            .ToRegister("AddTelemetryConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
            .HasDependency(this));

        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.ApplicationInsights:
                this.ApplyAppSetting("ApplicationInsights:ConnectionString", "Insert Application Insights Connection String Here");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

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