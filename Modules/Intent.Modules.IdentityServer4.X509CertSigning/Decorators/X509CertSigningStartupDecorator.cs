using Intent.Engine;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.AspNetCore.Templates.Startup;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class X509CertSigningStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "IdentityServer4.X509CertSigning.X509CertSigningStartupDecorator";

        public X509CertSigningStartupDecorator(StartupTemplate startupTemplate)
        {
            this.Priority = -8;
        }

        public override string ConfigureServices()
        {
            return "idServerBuilder.AddSigningCredential(CertificateRepo.GetUsingOptions(Configuration));";
        }
    }
}
