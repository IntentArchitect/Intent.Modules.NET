using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    public class BearerTokenStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override string DecoratorId => BearerTokenStartupDecorator.DecoratorId;

        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new BearerTokenStartupDecorator(application);
        }
    }
}
