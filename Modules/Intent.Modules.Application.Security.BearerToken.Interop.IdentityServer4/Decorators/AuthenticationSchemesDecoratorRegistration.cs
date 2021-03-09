using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    public class AuthenticationSchemesDecoratorRegistration : DecoratorRegistration<ControllerTemplate, ControllerDecorator>
    {
        public override string DecoratorId => AuthenticationSchemesDecorator.DecoratorId;

        public override ControllerDecorator CreateDecoratorInstance(ControllerTemplate template, IApplication application)
        {
            return new AuthenticationSchemesDecorator();
        }
    }
}
