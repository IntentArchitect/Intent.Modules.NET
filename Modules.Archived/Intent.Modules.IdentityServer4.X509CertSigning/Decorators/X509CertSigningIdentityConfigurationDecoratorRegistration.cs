using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [Description(X509CertSigningIdentityConfigurationDecorator.DecoratorId)]
    public class X509CertSigningIdentityConfigurationDecoratorRegistration : DecoratorRegistration<IdentityServerConfigurationTemplate, IdentityConfigurationDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override IdentityConfigurationDecorator CreateDecoratorInstance(IdentityServerConfigurationTemplate template, IApplication application)
        {
            return new X509CertSigningIdentityConfigurationDecorator(template, application);
        }

        public override string DecoratorId => X509CertSigningIdentityConfigurationDecorator.DecoratorId;
    }
}