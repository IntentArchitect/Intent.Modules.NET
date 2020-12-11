using Intent.Engine;
using Intent.Modules.Common.Registrations;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.IdentityServer4.InMemoryStore.Decorators;
using Intent.Modules.IdentityServer4.Selfhost.Templates.Startup;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [Description(X509CertSigningStartupDecorator.Identifier)]
    public class X509CertSigningStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new X509CertSigningStartupDecorator(template);
        }

        public override string DecoratorId => X509CertSigningStartupDecorator.Identifier;
    }
}
