using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MultiTenancyStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Fully)]
        public MultiTenancyStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string ConfigureServices()
        {
            return @"services.ConfigureMultiTenancy(Configuration);";
        }

        public override string Configuration()
        {
            return @"
            app.UseMultiTenant();";
        }
    }
}