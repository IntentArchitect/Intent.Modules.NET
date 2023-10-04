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
            .AddClass("OpenTelemetryConfiguration")
            .OnBuild(file =>
            {
                var priClass = file.Classes.First();
                priClass.Static();

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
                            .AddArgument(GetTracingInstrumentationStatements(file))
                            .WithoutSemicolon()
                            .AddMetadata("telemetry-tracing", true))
                        .AddMetadata("telemetry-config", true));
                    method.AddStatement("return services;");
                });

                if (ExecutionContext.Settings.GetOpenTelemetry().CaptureLogs())
                {
                    file.AddUsing("Microsoft.Extensions.Logging");
                    file.AddUsing("Microsoft.Extensions.Hosting");
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
                                .AddStatements(GetLoggingExporterStatements(file))
                                .AddStatements($@"
                                    options.IncludeFormattedMessage = true;
                                    options.IncludeScopes = true;
                                    options.ParseStateValues = true;")));
                    });
                }
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

    private CSharpStatement GetTracingInstrumentationStatements(CSharpFile file)
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

        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                AddNugetDependency(NugetPackages.AzureMonitorOpenTelemetryExporter);
                file.AddUsing("Azure.Monitor.OpenTelemetry.Exporter");
                traceChain.AddChainStatement(new CSharpInvocationStatement("AddAzureMonitorTraceExporter")
                    .WithoutSemicolon()
                    .AddArgument(new CSharpLambdaBlock("options")
                        .AddStatement(@"options.ConnectionString = configuration[""ApplicationInsights:ConnectionString""];")));
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                AddNugetDependency(NugetPackages.OpenTelemetryExporterConsole);
                file.AddUsing("OpenTelemetry.Trace");
                traceChain.AddChainStatement(new CSharpInvocationStatement("AddConsoleExporter").WithoutSemicolon());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new CSharpLambdaBlock("trace").WithExpressionBody(traceChain);
    }
    
    private IEnumerable<CSharpStatement> GetLoggingExporterStatements(CSharpFile file)
    {
        switch (ExecutionContext.Settings.GetOpenTelemetry().Export().AsEnum())
        {
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                file.AddUsing("OpenTelemetry.Logs");
                yield return new CSharpInvocationStatement("options.AddConsoleExporter");
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                yield return new CSharpInvocationStatement("options.AddAzureMonitorLogExporter")
                    .AddArgument(new CSharpLambdaBlock("x")
                        .AddStatement($@"x.ConnectionString = context.Configuration[""ApplicationInsights:ConnectionString""];"));
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
            case Settings.OpenTelemetry.ExportOptionsEnum.AzureApplicationInsights:
                this.ApplyAppSetting("ApplicationInsights:ConnectionString",
                    "Insert Application Insights Connection String Here");
                break;
            case Settings.OpenTelemetry.ExportOptionsEnum.Console:
                // NOOP
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