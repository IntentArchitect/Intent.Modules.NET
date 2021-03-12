using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityUserDecorator : StartupDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EF.IdentityUserDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        public IdentityUserDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -10;
        }

        public override string ConfigureServices()
        {
            var dbContextTemplate = _application.FindTemplateInstance<IClassProvider>(DbContextTemplate.TemplateId);

            return $@"
services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<{dbContextTemplate.FullTypeName()}>()
    ;";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Identity",
                "Microsoft.AspNetCore.Identity.EntityFrameworkCore"
            };
        }
    }
}