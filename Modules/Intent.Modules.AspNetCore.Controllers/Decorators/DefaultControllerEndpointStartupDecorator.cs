using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DefaultControllerEndpointStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.DefaultControllerEndpointStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;
        private bool _overrideDefaultController;

        [IntentManaged(Mode.Merge)]
        public DefaultControllerEndpointStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            //application.EventDispatcher.Subscribe<OverrideDefaultControllerEvent>(evt =>
            //{
            //    _overrideDefaultController = true;
            //});
            Priority = -30;
        }

        public override string ConfigureServices()
        {
            if (_overrideDefaultController)
            {
                return string.Empty;
            }

            if (_template.IsNetCore2App())
            {
                return "services.AddMvc();";
            }

            return "services.AddControllers();";
        }

        public override string Configuration()
        {
            // No longer supporting .NET 2.x
            //if (_template.IsNetCore2App())
            //{
            //    return "app.UseMvc();";
            //}

            return string.Empty;
        }

        public override string EndPointMappings()
        {
            return @"endpoints.MapControllers();";
        }
    }
}