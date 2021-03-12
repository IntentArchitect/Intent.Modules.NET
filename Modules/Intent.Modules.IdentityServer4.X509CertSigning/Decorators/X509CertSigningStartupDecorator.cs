using Intent.Engine;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Decorators
{
    public class X509CertSigningStartupDecorator : StartupDecorator, IDecoratorExecutionHooks
    {
        public const string DecoratorId = "IdentityServer4.X509CertSigning.X509CertSigningStartupDecorator";
        private StartupTemplate _template;
        private IApplication _application;

        public X509CertSigningStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            this.Priority = -8;
        }

        public void BeforeTemplateExecution()
        {
            _application.EventDispatcher.Publish(new CertificateSpecifiedEvent());
        }

        public override string ConfigureServices()
        {
            return "idServerBuilder.AddSigningCredential(CertificateRepo.GetUsingOptions(Configuration));";
        }
    }
}
