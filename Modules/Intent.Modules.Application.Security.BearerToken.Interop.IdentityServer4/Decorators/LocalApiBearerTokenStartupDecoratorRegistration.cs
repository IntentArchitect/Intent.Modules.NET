using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    public class LocalApiBearerTokenStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override string DecoratorId => LocalApiBearerTokenStartupDecorator.DecoratorId;

        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new LocalApiBearerTokenStartupDecorator(application);
        }
    }
}
