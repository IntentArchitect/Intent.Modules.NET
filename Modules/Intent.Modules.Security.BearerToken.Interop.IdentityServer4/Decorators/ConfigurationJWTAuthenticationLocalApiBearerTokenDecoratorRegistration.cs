using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    [Description(ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator.DecoratorId)]
    public class ConfigurationJWTAuthenticationLocalApiBearerTokenDecoratorRegistration : DecoratorRegistration<ConfigurationJWTAuthenticationTemplate, JWTAuthenticationDecorator>
    {
        public override JWTAuthenticationDecorator CreateDecoratorInstance(ConfigurationJWTAuthenticationTemplate template, IApplication application)
        {
            return new ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator(template, application);
        }

        public override string DecoratorId => ConfigurationJWTAuthenticationLocalApiBearerTokenDecorator.DecoratorId;
    }
}
