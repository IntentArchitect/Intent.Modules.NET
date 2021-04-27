using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DefaultControllerServicesStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.DefaultControllerServicesStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;
        private bool _overrideDefaultController;

        [IntentManaged(Mode.Merge)]
        public DefaultControllerServicesStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -30;
            application.EventDispatcher.Subscribe<OverrideDefaultControllerEvent>(evt =>
            {
                _overrideDefaultController = true;
            });
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

    }
}