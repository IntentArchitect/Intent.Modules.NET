using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.IdentityServer4.Identity.EFCore.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.Identity.EFCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class IdentityDbContextDecorator : DecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.Identity.EFCore.IdentityDbContextDecorator";


        [IntentManaged(Mode.Fully)]
        private readonly ICSharpFileBuilderTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public IdentityDbContextDecorator(ICSharpFileBuilderTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.FulfillsRole("Infrastructure.Data.IdentityDbContext");
            _template.AddNugetDependency(NugetPackages.IdentityServer4EntityFramework);
            _template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(_template.OutputTarget.GetProject()));

            _template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                _template.AddUsing("Microsoft.AspNetCore.Identity.EntityFrameworkCore");
                @class.WithBaseType($"IdentityDbContext<{_template.GetIdentityUserClass()}>");
            });
        }
    }

    public static class IdentityHelperExtensions
    {
        public static string GetIdentityUserClass(this ICSharpTemplate template)
        {
            var identityModel = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels()
                .SingleOrDefault(x => x.HasIdentityUser());
            var identityUserClass = identityModel != null
                ? template.GetTypeName("Domain.Entity", identityModel)
                : template.GetTypeName("Domain.IdentityUser", TemplateDiscoveryOptions.DoNotThrow);

            return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
        }

        public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
        {
            return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
        }
    }
}
