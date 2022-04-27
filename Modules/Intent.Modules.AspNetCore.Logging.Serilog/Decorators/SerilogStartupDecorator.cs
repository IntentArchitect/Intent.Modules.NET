using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SerilogStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Modules.AspNetCore.Logging.Serilog.SerilogStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SerilogStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            _template.AddNugetDependency(NugetPackages.SerilogAspNetCore);
            _template.AddUsing("Serilog");
        }

        public override string Configuration()
        {
            return "app.UseSerilogRequestLogging();";
        }

        public override int Priority { get; set; } = -100;
    }
}