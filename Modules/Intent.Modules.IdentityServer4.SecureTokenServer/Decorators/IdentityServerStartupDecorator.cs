using Intent.Modules.AspNetCore.Templates.Startup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityServerStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "IdentityServer4.SecureTokenServer.IdentityServerStartupDecorator";

        public IdentityServerStartupDecorator()
        {
            Priority = -9;
        }

        public override string ConfigureServices()
        {
            return "var idServerBuilder = services.AddIdentityServer();";
        }
    }
}
