using Intent.Modules.AspNetCore.Templates.Startup;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityConfigurationStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.SecureTokenServer.IdentityConfigurationStartupDecorator";

        private readonly StartupTemplate _template;

        public IdentityConfigurationStartupDecorator(StartupTemplate template)
        {
            _template = template;
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
