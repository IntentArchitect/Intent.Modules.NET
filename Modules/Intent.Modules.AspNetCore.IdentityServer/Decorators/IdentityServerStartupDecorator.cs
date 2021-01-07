using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityServerStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "AspNetCore.IdentityServer.IdentityServerStartupDecorator";

        private readonly StartupTemplate _template;

        public IdentityServerStartupDecorator(StartupTemplate template)
        {
            _template = template;
        }

    }
}