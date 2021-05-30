using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class StaticFilesStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.StaticFilesStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public StaticFilesStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -15;
        }

        public override string Configuration()
        {
            return "app.UseStaticFiles();";
        }
    }
}