using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    [Description(AuthenticationSchemesDecorator.DecoratorId)]
    public class AuthenticationSchemesDecoratorRegistration : DecoratorRegistration<ControllerTemplate, ControllerDecorator>
    {
        public override ControllerDecorator CreateDecoratorInstance(ControllerTemplate template, IApplication application)
        {
            return new AuthenticationSchemesDecorator(template, application);
        }

        public override string DecoratorId => AuthenticationSchemesDecorator.DecoratorId;
    }
}
