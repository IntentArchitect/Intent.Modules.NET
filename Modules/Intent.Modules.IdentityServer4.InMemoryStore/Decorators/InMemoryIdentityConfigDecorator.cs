using Intent.Modules.IdentityServer4.Selfhost.Templates.IdentityConfig;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class InMemoryIdentityConfigDecorator : IdentityConfigDecorator
    {
        public const string Identifier = "IdentityServer4.InMemoryStore.IdentityConfigDecorator";

        public InMemoryIdentityConfigDecorator(IdentityConfigTemplate template)
        {
            this.Priority = 1;
        }

        public override EntityCollection GetClients()
        {
            return new EntityCollection()
            {
                new Entity
                {
                    { "ClientId", @"$""{ApiResourceName}_pwd_client""" } ,
                    { "AllowedGrantTypes", "GrantTypes.ResourceOwnerPassword"},
                    { "RequireClientSecret", "false"},
                    { "AllowedScopes", @"{ ""openid"", ""profile"", ""email"", ApiResourceName }"}
                }
            };
        }
    }
}
