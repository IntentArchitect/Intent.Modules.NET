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
using Intent.Modules.Common.CSharp.VisualStudio;
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
    public OpenTelemetryConfigurationTemplate(IOutputTarget outputTarget, object model = null)
        : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.OpenTelemetry(OutputTarget));
        AddNugetDependency(NugetPackages.OpenTelemetryExtensionsHosting(OutputTarget));

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("OpenTelemetry.Resources")
            .AddUsing("OpenTelemetry.Trace")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddClass("OpenTelemetryConfiguration", @class =>
            {
                @class.Static();
                AddTracingAndMetricsTelemetryConfiguration(@class);
                AddLoggingTelemetryConfiguration(@class);
            });
    }

    private void AddTracingAndMetricsTelemetryConfiguration(CSharpClass priClass)
    {
        priClass.AddMethod("IServiceCollection", "AddTelemetryConfiguration", method =>
        {
            method.Static();
            method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
            method.AddParameter("IConfiguration", "configuration");

            if (ExecutionContext.Settings.GetOpenTelemetry().Export().IsAzureMonitorOpentelemetryDistro())
            {
                AddNugetDependency(NugetPackages.AzureMonitorOpenTelemetryAspNetCore(OutputTarget));
                AddUsing("Azure.Monitor.OpenTelemetry.AspNetCore");
                
                method.AddInvocationStatement("services.AddOpenTelemetry", main => main
                    .AddInvocation("UseAzureMonitor", inv => inv
                        .AddArgument(new CSharpLambdaBlock("opt")
                            .AddStatement(@"opt.ConnectionString = configuration[""ApplicationInsights:ConnectionString""];")))
                    .AddInvocation("ConfigureResource", inv => inv
                        .AddArgument(GetResourceConfigurationStatement())
                        .OnNewLine()
                        .AddMetadata("telemetry-resource", true)));
            }
            else
            {
                method.AddInvocationStatement("services.AddOpenTelemetry", main => main
                    .AddInvocation("ConfigureResource", inv => inv
                        .AddArgument(GetResourceConfigurationStatement())
                        .OnNewLine()
                        .AddMetadata("telemetry-resource", true))
                    .Condition(ExecutionContext.Settings.GetOpenTelemetry().CaptureTraces(), x => x.AddInvocation("WithTracing", inv => inv
                        .AddArgument(GetTracingInstrumentationStatements())
                        .OnNewLine()
                        .AddMetadata("telemetry-tracing", true)))
                    .Condition(ExecutionContext.Settings.GetOpenTelemetry().CaptureMetrics(), x => x.AddInvocation("WithMetrics", inv => inv
                        .AddArgument(GetMetricsInstrumentationStatements())
                        .OnNewLine()
                        .AddMetadata("telemetry-metrics", true)))
                    .AddMetadata("telemetry-config", true));
            }

            method.AddStatement("return services;");
        });
    }

    private void AddLoggingTelemetryConfiguration(CSharpClass priClass)
    {
        if (!ExecutionContext.Settings.GetOpenTelemetry().CaptureLogs() || ExecutionContext.Settings.GetOpenTelemetry().Export().IsAzureMonitorOpentelemetryDistro())
        {
            return;
        }

        AddUsing("Microsoft.Extensions.Logging");
        AddUsing("Microsoft.Extensions.Hosting");
        priClass.AddMethod("ILoggingBuilder", "AddTelemetryConfiguration", method =>
        {
            method.Static();
            method.AddParameter("ILoggingBuilder", "logBuilder", param => param.WithThisModifier());
            method.AddParameter("HostBuilderContext", "context");

            method.AddInvocationStatement($"return logBuilder.AddOpenTelemetry", addOpenTel => addOpenTel
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddInvocationStatement("options.SetResourceBuilder", setBuilder => setBuilder
                        .AddArgument(new CSharpStatement("ResourceBuilder")
                            .AddInvocation("CreateDefault", inv => inv.OnNewLine())
                            .AddInvocation($@"AddService", inv => inv.AddArgument(@"context.Configuration[""OpenTelemetry:ServiceName""]!").OnNewLine().WithoutSemicolon()))
                    )
                    .AddStatements(GetLoggingExporterStatements())
                    .AddStatements("""
                                   options.IncludeFormattedMessage = true;
                                   options.IncludeScopes = true;
                                   options.ParseStateValues = true;
                                   """)
                ));
        });
    }

    private CSharpStatement GetResourceConfigurationStatement()
    {
        return new CSharpLambdaBlock("res")
            .WithExpressionBody(new CSharpStatement("res")
                .AddInvocation("AddService", inv => inv
                    .AddArgument(@"serviceName: configuration[""OpenTelemetry:ServiceName""]!")
                    .AddArgument(@"serviceInstanceId: configuration.GetValue<string?>(""OpenTelemetry:ServiceInstanceId"")")
                    .OnNewLine())
                .AddInvocation("AddTelemetrySdk", inv => inv.OnNewLine())
                .AddInvocation("AddEnvironmentVariableDetector", inv => inv.OnNewLine()).WithoutSemicolon()
            );
    }

    private CSharpStatement GetTracingInstrumentationStatements()
    {
        AddUsing("OpenTelemetry.Trace");

        var traceChain = new CSharpStatement("trace");

        // TEMP FIX - also see TelemetryConfiguratorExtension
        if (ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Eventing.MassTransit"))
        {
            traceChain = traceChain.AddInvocation("AddSource", inv => inv.AddArgument(@"""MassTransit""").OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().ASPNETCoreInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationAspNetCore(OutputTarget));
            traceChain = traceChain.AddInvocation("AddAspNetCoreInstrumentation", inv => inv.OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().HTTPInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationHttp(OutputTarget));
            traceChain = traceChain.AddInvocation("AddHttpClientInstrumentation", inv => inv.OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().SQLInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationSqlClient(OutputTarget));
            traceChain = traceChain.AddInvocation("AddSqlClientInstrumentation", inv => inv.OnNewLine());
        }

        var finalChain = AddExporterConfiguration(traceChain, ExporterType.Trace);

        return new CSharpLambdaBlock("trace").WithExpressionBody(finalChain.WithoutSemicolon());
    }

    private CSharpStatement GetMetricsInstrumentationStatements()
    {
        AddUsing("OpenTelemetry.Metrics");

        var traceChain = new CSharpStatement("metrics");

        if (ExecutionContext.Settings.GetOpenTelemetry().ASPNETCoreInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationAspNetCore(OutputTarget));
            traceChain = traceChain.AddInvocation("AddAspNetCoreInstrumentation", inv => inv.OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().HTTPInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationHttp(OutputTarget));
            traceChain = traceChain.AddInvocation("AddHttpClientInstrumentation", inv => inv.OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().ProcessInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationProcess(OutputTarget));
            traceChain = traceChain.AddInvocation("AddProcessInstrumentation", inv => inv.OnNewLine());
        }

        if (ExecutionContext.Settings.GetOpenTelemetry().NETRuntimeInstrumentation())
        {
            AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationRuntime(OutputTarget));
            traceChain = traceChain.AddInvocation("AddRuntimeInstrumentation", inv => inv.OnNewLine());
        }

        var finalChain = AddExporterConfiguration(traceChain, ExporterType.Metric);

        return new CSharpLambdaBlock("metrics").WithExpressionBody(finalChain.WithoutSemicolon());
    }

    private enum ExporterType
    {
        Trace,
        Metric
    }

    private CSharpInvocationStatement AddExporterConfiguration(CSharpStatement configChain, ExporterType exporterType)
    {
        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                {
                    AddNugetDependency(NugetPackages.OpenTelemetryExporterConsole(OutputTarget));
                    
                    return configChain.AddInvocation("AddConsoleExporter", inv => inv.OnNewLine());
                }
            case Settings.OpenTelemetry.ExportOptionsEnum.OpenTelemetryProtocol:
                {
                    AddNugetDependency(NugetPackages.OpenTelemetryExporterOpenTelemetryProtocol(OutputTarget));
                    
                    return configChain.AddInvocation("AddOtlpExporter", inv => inv.OnNewLine()
                        .AddArgument(new CSharpLambdaBlock("opt")
                            .AddStatement($@"opt.Endpoint = configuration.GetValue<{UseType("System.Uri")}>(""open-telemetry-protocol:endpoint"")!;")
                            .AddStatement($@"opt.Protocol = configuration.GetValue<{UseType("OpenTelemetry.Exporter.OtlpExportProtocol")}>(""open-telemetry-protocol:protocol"");")));
                }
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                {
                    AddNugetDependency(NugetPackages.AzureMonitorOpenTelemetryExporter(OutputTarget));
                    
                    AddUsing("Azure.Monitor.OpenTelemetry.Exporter");

                    return configChain.AddInvocation($"AddAzureMonitor{exporterType}Exporter", inv => inv.OnNewLine()
                        .AddArgument(new CSharpLambdaBlock("opt").AddStatement(@"opt.ConnectionString = configuration[""ApplicationInsights:ConnectionString""];")));
                }
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureMonitorOpentelemetryDistro:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerable<CSharpStatement> GetLoggingExporterStatements()
    {
        AddUsing("OpenTelemetry.Logs");

        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                yield return new CSharpInvocationStatement("options.AddConsoleExporter");
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.OpenTelemetryProtocol:
                yield return new CSharpInvocationStatement("options.AddOtlpExporter")
                    .AddArgument(new CSharpLambdaBlock("opt")
                        .AddStatement($@"opt.Endpoint = context.Configuration.GetValue<{UseType("System.Uri")}>(""open-telemetry-protocol:endpoint"")!;")
                        .AddStatement($@"opt.Protocol = context.Configuration.GetValue<{UseType("OpenTelemetry.Exporter.OtlpExportProtocol")}>(""open-telemetry-protocol:protocol"");"));
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                yield return new CSharpInvocationStatement("options.AddAzureMonitorLogExporter")
                    .AddArgument(new CSharpLambdaBlock("opt").AddStatement($@"opt.ConnectionString = context.Configuration[""ApplicationInsights:ConnectionString""];"));
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureMonitorOpentelemetryDistro:

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
            .ToRegister("AddTelemetryConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
            .HasDependency(this));

        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                // NOOP
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.OpenTelemetryProtocol:
                this.ApplyAppSetting("open-telemetry-protocol", new
                {
                    endpoint = "",
                    protocol = "Grpc"
                });
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                this.ApplyAppSetting("ApplicationInsights:ConnectionString",
                    "Insert Application Insights Connection String Here");
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureMonitorOpentelemetryDistro:
                this.ApplyAppSetting("ApplicationInsights:ConnectionString",
                    "Insert Application Insights Connection String Here");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        this.ApplyAppSetting("OpenTelemetry", new
        {
            ServiceName = Project.ApplicationName(),
            ServiceInstanceId = (string)null
        });
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

[IntentIgnore]
internal static class ConditionalExtensions
{
    public static T Condition<T>(this T source, bool condition, Func<T, T> func)
    {
        if (condition)
        {
            return func(source);
        }
        return source;
    }
}