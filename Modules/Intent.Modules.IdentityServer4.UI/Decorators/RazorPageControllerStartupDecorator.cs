using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class RazorPageControllerStartupDecorator : StartupDecorator, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.RazorPageControllerStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        public RazorPageControllerStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -30;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new OverrideDefaultControllerEvent());
        }

        public override string ConfigureServices()
        {
            return @"
services.AddControllersWithViews();
services.AddRazorPages();";
        }
    }
}