using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.Identity.EntityFrameworkCore.Decorators
{
    public class IdentityDbContextDecoratorRegistration : DecoratorRegistration<DbContextDecoratorBase>
    {
        public override string DecoratorId => IdentityDbContextDecorator.DecoratorId;

        public override DbContextDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new IdentityDbContextDecorator(application);
        }
    }
}
