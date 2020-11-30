using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Registrations;

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [Description(SwashbuckleCoreWebStartupDecorator.Identifier)]
    public class SwashbuckleCoreWebStartupDecoratorRegistration : DecoratorRegistration<StartupDecorator>
    {
        public override string DecoratorId => SwashbuckleCoreWebStartupDecorator.Identifier;

        public override StartupDecorator CreateDecoratorInstance(IApplication application)
        {
            return new SwashbuckleCoreWebStartupDecorator();
        }
    }
}
