using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.Selfhost.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    [Description(InMemoryStartupDecorator.Identifier)]
    public class InMemoryStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new InMemoryStartupDecorator(template);
        }

        public override string DecoratorId => InMemoryStartupDecorator.Identifier;
    }
}