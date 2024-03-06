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
    public OpenTelemetryConfigurationTemplate(IOutputTarget outputTarget, object model = null)
        : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.OpenTelemetry);
        AddNugetDependency(NugetPackages.OpenTelemetryExtensionsHosting);
        AddNugetDependency(NugetPackages.OpenTelemetryInstrumentationAspNetCore);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("OpenTelemetry.Resources")
            .AddUsing("OpenTelemetry.Trace")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddClass("OpenTelemetryConfiguration", @class =>
            {
                @class.Static();
                AddTracingTelemetryConfiguration(@class);
                AddLoggingTelemetryConfiguration(@class);
            });
    }

    private void AddTracingTelemetryConfiguration(CSharpClass priClass)
    {
        priClass.AddMethod("IServiceCollection", "AddTelemetryConfiguration", method =>
        {
            method.Static();
            method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
            method.AddParameter("IConfiguration", "configuration");
            method.AddMethodChainStatement("services.AddOpenTelemetry()", main => main
                .AddChainStatement(new CSharpInvocationStatement("ConfigureResource")
                    .AddArgument(GetResourceConfigurationStatement())
                    .WithoutSemicolon()
                    .AddMetadata("telemetry-resource", true))
                .AddChainStatement(new CSharpInvocationStatement("WithTracing")
                    .AddArgument(GetTracingInstrumentationStatements())
                    .WithoutSemicolon()
                    .AddMetadata("telemetry-tracing", true))
                .AddMetadata("telemetry-config", true));
            method.AddStatement("return services;");
        });
    }

    private void AddLoggingTelemetryConfiguration(CSharpClass priClass)
    {
        if (!ExecutionContext.Settings.GetOpenTelemetry().CaptureLogs())
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
                        .AddArgument(new CSharpMethodChainStatement("ResourceBuilder")
                            .WithoutSemicolon()
                            .AddChainStatement("CreateDefault()")
                            .AddChainStatement($@"AddService(""{Project.ApplicationName()}"")")))
                    .AddStatements(GetLoggingExporterStatements())
                    .AddStatements($@"
                                    options.IncludeFormattedMessage = true;
                                    options.IncludeScopes = true;
                                    options.ParseStateValues = true;")));
        });
    }

    private CSharpStatement GetResourceConfigurationStatement()
    {
        return new CSharpLambdaBlock("res")
            .WithExpressionBody(new CSharpMethodChainStatement("res")
                .AddChainStatement($@"AddService(""{Project.ApplicationName()}"")")
                .AddChainStatement("AddTelemetrySdk()")
                .AddChainStatement("AddEnvironmentVariableDetector()"));
    }

    private CSharpStatement GetTracingInstrumentationStatements()
    {
        var traceChain = new CSharpMethodChainStatement("trace")
            .WithoutSemicolon()
            .AddChainStatement("AddAspNetCoreInstrumentation()");

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

        AddExporterConfiguration(traceChain);

        return new CSharpLambdaBlock("trace").WithExpressionBody(traceChain);
    }

    private void AddExporterConfiguration(CSharpMethodChainStatement configChain)
    {
        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                AddNugetDependency(NugetPackages.OpenTelemetryExporterConsole);
                AddUsing("OpenTelemetry.Trace");
                configChain.AddChainStatement(new CSharpInvocationStatement("AddConsoleExporter").WithoutSemicolon());
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.OpenTelemetryProtocol:
                AddNugetDependency(NugetPackages.OpenTelemetryExporterOpenTelemetryProtocol);
                AddUsing("OpenTelemetry.Trace");
                configChain.AddChainStatement(new CSharpInvocationStatement("AddOtlpExporter")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("opt")
                        .AddStatement($@"opt.Endpoint = configuration.GetValue<{UseType("System.Uri")}>(""open-telemetry-protocol:endpoint"");")
                        .AddStatement($@"opt.Protocol = configuration.GetValue<{UseType("OpenTelemetry.Exporter.OtlpExportProtocol")}>(""open-telemetry-protocol:protocol"");")));
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                AddNugetDependency(NugetPackages.AzureMonitorOpenTelemetryExporter);
                AddUsing("Azure.Monitor.OpenTelemetry.Exporter");
                configChain.AddChainStatement(new CSharpInvocationStatement("AddAzureMonitorTraceExporter")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("opt")
                        .AddStatement(@"opt.ConnectionString = configuration[""ApplicationInsights:ConnectionString""];")));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerable<CSharpStatement> GetLoggingExporterStatements()
    {
        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                AddUsing("OpenTelemetry.Logs");
                yield return new CSharpInvocationStatement("options.AddConsoleExporter");
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.OpenTelemetryProtocol:
                AddUsing("OpenTelemetry.Logs");
                yield return new CSharpInvocationStatement("options.AddOtlpExporter")
                    .AddArgument(new CSharpLambdaBlock("opt")
                        .AddStatement($@"opt.Endpoint = context.Configuration.GetValue<{UseType("System.Uri")}>(""open-telemetry-protocol:endpoint"");")
                        .AddStatement($@"opt.Protocol = context.Configuration.GetValue<{UseType("OpenTelemetry.Exporter.OtlpExportProtocol")}>(""open-telemetry-protocol:protocol"");"));
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                yield return new CSharpInvocationStatement("options.AddAzureMonitorLogExporter")
                    .AddArgument(new CSharpLambdaBlock("opt")
                        .AddStatement($@"opt.ConnectionString = context.Configuration[""ApplicationInsights:ConnectionString""];"));
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
                    endpoint = "http://localhost:4317",
                    protocol = "Grpc"
                });
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                this.ApplyAppSetting("ApplicationInsights:ConnectionString",
                    "Insert Application Insights Connection String Here");
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