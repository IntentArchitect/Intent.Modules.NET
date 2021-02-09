using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    public class X509CertSigningAppSettingsDecorator : AppSettingsDecorator
    {
        public const string Identifier = "IdentityServer4.X509CertSigning.AppSettingsDecorator";

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
