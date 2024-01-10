using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.IdentityServer4.Identity.EFCore.FactoryExtensions;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityUserDecorator : IdentityConfigurationDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.IdentityUserDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly IdentityServerConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public IdentityUserDecorator(IdentityServerConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = -50;
        }

        public override string ConfigureServices()
        {
            var dbContextTemplate = _application.FindTemplateInstance<IClassProvider>(DbContextTemplate.TemplateId);

            return $@"
            services.AddIdentity<{_template.GetIdentityUserClass()}, {_template.GetIdentityRoleClass()}>()
                .AddEntityFrameworkStores<{_template.GetTypeName("Infrastructure.Data.IdentityDbContext")}>();";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.AspNetCore.Identity.EntityFrameworkCore"
            };
        }
    }
}