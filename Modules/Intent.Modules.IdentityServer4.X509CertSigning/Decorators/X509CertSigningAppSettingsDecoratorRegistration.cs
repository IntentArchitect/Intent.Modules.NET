using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    public class X509CertSigningAppSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsDecorator>
    {
        public override string DecoratorId => X509CertSigningAppSettingsDecorator.Identifier;

        public override AppSettingsDecorator CreateDecoratorInstance(IApplication application)
        {
            return new X509CertSigningAppSettingsDecorator();
        }
    }
}
