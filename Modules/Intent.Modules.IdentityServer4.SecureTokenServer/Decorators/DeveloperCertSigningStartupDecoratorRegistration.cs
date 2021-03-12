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

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Decorators
{
    [Description(DeveloperCertSigningStartupDecorator.DecoratorId)]
    public class DeveloperCertSigningStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new DeveloperCertSigningStartupDecorator(template, application);
        }

        public override string DecoratorId => DeveloperCertSigningStartupDecorator.DecoratorId;
    }
}
