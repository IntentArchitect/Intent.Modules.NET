using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class StartupDefaultMSALDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.MSAL.StartupDefaultMSALDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public StartupDefaultMSALDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -10;
            _template.AddTemplateDependency(ConfigurationMSALAuthenticationTemplate.TemplateId);
        }

        public override string Configuration()
        {
            return "app.UseAuthentication();";
        }

        public override string ConfigureServices()
        {
            return @"services.ConfigureMSALSecurity(Configuration);";
        }
    }
}