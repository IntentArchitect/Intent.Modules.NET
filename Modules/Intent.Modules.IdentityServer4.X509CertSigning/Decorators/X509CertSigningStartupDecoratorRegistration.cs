using Intent.Engine;
using Intent.Modules.Common.Registrations;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.IdentityServer4.InMemoryStore.Decorators;
using Intent.Modules.AspNetCore.Templates.Startup;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [Description(X509CertSigningStartupDecorator.DecoratorId)]
    public class X509CertSigningStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new X509CertSigningStartupDecorator(template, application);
        }

        public override string DecoratorId => X509CertSigningStartupDecorator.DecoratorId;
    }
}
