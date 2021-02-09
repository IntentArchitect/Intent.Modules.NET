using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.IdentityServer4.Selfhost.Templates.Startup;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class X509CertSigningStartupDecorator : StartupDecorator
    {
        public const string Identifier = "IdentityServer4.X509CertSigning.X509CertSigningStartupDecorator";

        public X509CertSigningStartupDecorator(StartupTemplate startupTemplate)
        {
            this.Priority = 1;
        }

        public override IReadOnlyCollection<string> GetServicesConfigurationStatements()
        {
            return new[]
            {
                @"AddSigningCredential(CertificateRepo.GetUsingOptions(Configuration))"
            };
        }
    }
}
