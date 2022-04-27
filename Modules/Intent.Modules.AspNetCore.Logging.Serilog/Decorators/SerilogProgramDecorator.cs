using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SerilogProgramDecorator : ProgramDecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Modules.AspNetCore.Logging.Serilog.SerilogProgramDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ProgramTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SerilogProgramDecorator(ProgramTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            _template.AddNugetDependency(NugetPackages.SerilogAspNetCore);
            _template.AddUsing("Serilog");
            _template.AddUsing("Serilog.Events");
        }

        public override void BeforeCallBuilder()
        {
            _template.WriteLine($@"             Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override(""Microsoft"", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {{
                Log.Information(""Starting web host"");");

            _template.PushIndent("    ");
        }

        public override void AfterCallBuilder()
        {
            _template.PopIndent();

            _template.WriteLine($@"             }}
            catch ({_template.UseType("System.Exception")} ex)
            {{
                Log.Fatal(ex, ""Host terminated unexpectedly"");
            }}
            finally
            {{
                Log.CloseAndFlush();
            }}");
        }

        public override IEnumerable<string> GetFluentBuilderLines()
        {
            yield return @".UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())";
        }
    }
}