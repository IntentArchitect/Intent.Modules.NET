using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [Description(IdentityDbContextDecorator.DecoratorId)]
    public class IdentityDbContextDecoratorRegistration : DecoratorRegistration<DbContextTemplate, ITemplateDecorator>
    {
        public override ITemplateDecorator CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new IdentityDbContextDecorator(template, application);
        }

        public override string DecoratorId => IdentityDbContextDecorator.DecoratorId;
    }
}
