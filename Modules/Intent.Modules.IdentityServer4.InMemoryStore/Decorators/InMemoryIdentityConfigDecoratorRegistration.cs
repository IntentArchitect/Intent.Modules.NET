using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.Selfhost.Templates.IdentityConfig;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    [Description(InMemoryIdentityConfigDecorator.Identifier)]
    public class InMemoryIdentityConfigDecoratorRegistration : DecoratorRegistration<IdentityConfigTemplate, IdentityConfigDecorator>
    {
        public override IdentityConfigDecorator CreateDecoratorInstance(IdentityConfigTemplate template, IApplication application)
        {
            return new InMemoryIdentityConfigDecorator(template);
        }

        public override string DecoratorId => InMemoryIdentityConfigDecorator.Identifier;
    }
}