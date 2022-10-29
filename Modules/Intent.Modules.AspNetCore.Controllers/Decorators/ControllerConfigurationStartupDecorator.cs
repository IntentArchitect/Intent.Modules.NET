using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
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
                    .InsertStatement(0, "services.AddControllers();");
                @class.Methods.First(x => x.Name == "Configure")
                    .Statements.OfType<EndpointsStatement>().First()
                    .AddEndpointConfiguration("endpoints.MapControllers();");
            });
        }

        //public override string ConfigureServices()
        //{

        //    return "services.AddControllers();";
        //}

        //public override string Configuration()
        //{
        //    return string.Empty;
        //}

        //public override string EndPointMappings()
        //{
        //    return @"endpoints.MapControllers();";
        //}
    }
}