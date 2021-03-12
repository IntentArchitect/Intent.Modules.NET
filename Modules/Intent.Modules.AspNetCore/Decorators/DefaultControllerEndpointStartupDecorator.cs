using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DefaultControllerEndpointStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.DefaultControllerEndpointStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;
        private bool _overrideDefaultController;

        public DefaultControllerEndpointStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            application.EventDispatcher.Subscribe<OverrideDefaultControllerEvent>(evt =>
            {
                _overrideDefaultController = true;
            });
            Priority = 20;
        }

        public override string Configuration()
        {
            if (_overrideDefaultController)
            {
                return string.Empty;
            }

            if (_template.IsNetCore2App())
            {
                return "app.UseMvc();";
            }

            return @"
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});";
        }
    }
}