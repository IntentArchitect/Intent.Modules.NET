using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Application.Security.BearerToken.Decorators
{
    [Description(BearerTokenStartupDecorator.DecoratorId)]
    public class BearerTokenStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new BearerTokenStartupDecorator(template, application);
        }

        public override string DecoratorId => BearerTokenStartupDecorator.DecoratorId;
    }
}
