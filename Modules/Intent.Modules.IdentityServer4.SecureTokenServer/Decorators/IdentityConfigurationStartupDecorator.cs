using Intent.Modules.AspNetCore.Templates.Startup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityConfigurationStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "IdentityServer4.SecureTokenServer.IdentityConfigurationStartupDecorator";

        public IdentityConfigurationStartupDecorator()
        {
            Priority = -8;
        }

        public override string ConfigureServices()
        {
            return @"
idServerBuilder.AddInMemoryClients(Configuration.GetSection(""IdentityServer:Clients""))
    .AddInMemoryApiResources(Configuration.GetSection(""IdentityServer:ApiResources""))
    .AddInMemoryApiScopes(Configuration.GetSection(""IdentityServer:ApiScopes""))
    .AddInMemoryIdentityResources(Configuration.GetSection(""IdentityServer:IdentityResources""));";
        }
    }
}
