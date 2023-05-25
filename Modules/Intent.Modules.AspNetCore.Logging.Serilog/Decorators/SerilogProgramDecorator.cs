using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators;

[IntentManaged(Mode.Merge)]
public class SerilogProgramDecorator : ProgramDecoratorBase
{
    [IntentManaged(Mode.Fully)]
    public const string DecoratorId = "Intent.Modules.AspNetCore.Logging.Serilog.SerilogProgramDecorator";

    [IntentManaged(Mode.Fully)] private readonly ProgramTemplate _template;
    [IntentManaged(Mode.Fully)] private readonly IApplication _application;

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public SerilogProgramDecorator(ProgramTemplate template, IApplication application)
    {
        _template = template;
        _application = application;

        _template.AddNugetDependency(NugetPackages.SerilogAspNetCore);

        _template.CSharpFile.OnBuild(file =>
        {
            file.AddUsing("Serilog");
            file.AddUsing("Serilog.Events");

            var @class = file.Classes.First();
            var main = @class.FindMethod("Main");
            var hostRunStmt = main.FindStatement(stmt => stmt.HasMetadata("host-run"));
            hostRunStmt.Remove();
            
            main.AddMethodChainStatement("Log.Logger = new LoggerConfiguration()", stmt => stmt
                .AddChainStatement(@"MinimumLevel.Override(""Microsoft"", LogEventLevel.Information)")
                .AddChainStatement(@"Enrich.FromLogContext()")
                .AddChainStatement(@"WriteTo.Console()")
                .AddChainStatement(@"CreateBootstrapLogger()"));

            main.AddTryBlock(block =>
                block.AddStatement(@"Log.Information(""Starting web host"");")
                    .AddStatement(hostRunStmt));
            main.AddCatchBlock(_template.UseType("System.Exception"), "ex",
                block => block.AddStatement(@"Log.Fatal(ex, ""Host terminated unexpectedly"");"));
            main.AddFinallyBlock(block => block.AddStatement("Log.CloseAndFlush();"));

            var hostBuilder = @class.FindMethod("CreateHostBuilder");
            var hostBuilderChain = (CSharpMethodChainStatement)hostBuilder.Statements.First();
            hostBuilderChain.Statements.Last().InsertAbove(new CSharpInvocationStatement("UseSerilog")
                .WithoutSemicolon()
                .AddArgument(new CSharpLambdaBlock("(context, services, configuration)")
                    .WithExpressionBody(new CSharpMethodChainStatement("configuration")
                        .AddChainStatement("ReadFrom.Configuration(context.Configuration)")
                        .AddChainStatement("ReadFrom.Services(services)")
                        .AddChainStatement("Enrich.FromLogContext()")
                        .AddChainStatement("WriteTo.Console()"))));
        }, 10);
    }
}