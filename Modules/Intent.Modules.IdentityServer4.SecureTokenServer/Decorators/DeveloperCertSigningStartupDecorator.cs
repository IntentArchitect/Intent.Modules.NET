using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DeveloperCertSigningStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.DeveloperCertSigningStartupDecorator";

        private readonly StartupTemplate _template;
        private bool _hasCertificateSpecified;

        public DeveloperCertSigningStartupDecorator(StartupTemplate template, Engine.IApplication application)
        {
            _template = template;
            Priority = -8;
            application.EventDispatcher.Subscribe<CertificateSpecifiedEvent>(evt =>
            {
                _hasCertificateSpecified = true;
            });
        }

        public override string ConfigureServices()
        {
            if (_hasCertificateSpecified)
            {
                return string.Empty;
            }

            return @"idServerBuilder.AddDeveloperSigningCredential();";
        }
    }
}
