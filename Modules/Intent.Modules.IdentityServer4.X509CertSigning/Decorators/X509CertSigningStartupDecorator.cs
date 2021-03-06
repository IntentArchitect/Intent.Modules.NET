using Intent.Engine;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.IdentityServer4.SecureTokenServer.Contracts;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    public class X509CertSigningStartupDecorator : StartupDecorator, IDecoratorExecutionHooks
    {
        public const string DecoratorId = "IdentityServer4.X509CertSigning.X509CertSigningStartupDecorator";

        public X509CertSigningStartupDecorator(StartupTemplate startupTemplate, IApplication application)
        {
            this.Priority = -8;
            Application = application;
        }

        public IApplication Application { get; }

        public void BeforeTemplateExecution()
        {
            Application.EventDispatcher.Publish(new CertificateSpecifiedEvent());
        }

        public override string ConfigureServices()
        {
            return "idServerBuilder.AddSigningCredential(CertificateRepo.GetUsingOptions(Configuration));";
        }
    }
}
