using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class X509CertSigningAppSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.X509CertSigning.X509CertSigningAppSettingsDecorator";

        private readonly AppSettingsTemplate _template;

        public X509CertSigningAppSettingsDecorator(AppSettingsTemplate template)
        {
            _template = template;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("CertificateOptions", new
            {
                Store = new
                {
                    FindType = "FindBySubjectName",
                    FindValue = "localhost",
                    StoreName = "My",
                    StoreLocation = "LocalMachine"
                }
            });
        }
    }
}
