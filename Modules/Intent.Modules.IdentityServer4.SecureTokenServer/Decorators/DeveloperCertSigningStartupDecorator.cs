using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.IdentityServer4.SecureTokenServer.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class DeveloperCertSigningStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "IdentityServer4.SecureTokenServer.DeveloperCertSigningStartupDecorator";

        private bool _hasCertificateSpecified;

        public DeveloperCertSigningStartupDecorator(StartupTemplate template, Engine.IApplication application)
        {
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
