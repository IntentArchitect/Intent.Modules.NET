using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [Description(SwashbuckleCoreWebStartupDecorator.DecoratorId)]
    public class SwashbuckleCoreWebStartupDecoratorRegistration : DecoratorRegistration<StartupTemplate, StartupDecorator>
    {
        public override StartupDecorator CreateDecoratorInstance(StartupTemplate template, IApplication application)
        {
            return new SwashbuckleCoreWebStartupDecorator(template, application);
        }

        public override string DecoratorId => SwashbuckleCoreWebStartupDecorator.DecoratorId;
    }
}
