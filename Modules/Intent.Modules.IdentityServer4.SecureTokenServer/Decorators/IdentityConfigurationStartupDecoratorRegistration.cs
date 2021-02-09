using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    public class IdentityConfigurationStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override string DecoratorId => IdentityConfigurationStartupDecorator.DecoratorId;

        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new IdentityConfigurationStartupDecorator();
        }
    }
}
