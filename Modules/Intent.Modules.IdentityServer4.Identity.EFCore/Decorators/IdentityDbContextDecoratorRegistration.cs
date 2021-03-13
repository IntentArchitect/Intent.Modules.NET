using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [Description(IdentityDbContextDecorator.DecoratorId)]
    public class IdentityDbContextDecoratorRegistration : DecoratorRegistration<DbContextTemplate, DbContextDecoratorBase>
    {
        public override DbContextDecoratorBase CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new IdentityDbContextDecorator(template, application);
        }

        public override string DecoratorId => IdentityDbContextDecorator.DecoratorId;
    }
}
