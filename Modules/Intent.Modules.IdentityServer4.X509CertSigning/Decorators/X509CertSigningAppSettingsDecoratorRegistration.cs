using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [Description(X509CertSigningAppSettingsDecorator.DecoratorId)]
    public class X509CertSigningAppSettingsDecoratorRegistration : DecoratorRegistration<AppSettingsTemplate, AppSettingsDecorator>
    {
        public override AppSettingsDecorator CreateDecoratorInstance(AppSettingsTemplate template, IApplication application)
        {
            return new X509CertSigningAppSettingsDecorator(template, application);
        }

        public override string DecoratorId => X509CertSigningAppSettingsDecorator.DecoratorId;
    }
}
