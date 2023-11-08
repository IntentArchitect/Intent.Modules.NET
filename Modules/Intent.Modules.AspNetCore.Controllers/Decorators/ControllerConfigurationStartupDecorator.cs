using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ControllerConfigurationStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.ControllerConfigurationStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ControllerConfigurationStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                @class.Methods.First(x => x.Name == "ConfigureServices")
                    .InsertStatement(0, new CSharpInvocationStatement("services.AddControllers").WithArgumentsOnNewLines(), s => s.AddMetadata("configure-services-controllers-generic", true));
                @class.Methods.First(x => x.Name == "Configure")
                    .Statements.OfType<EndpointsStatement>().First()
                    .AddEndpointConfiguration(new CSharpStatement("endpoints.MapControllers();").AddMetadata("configure-endpoints-controllers-generic", true));
            });
        }
    }
}