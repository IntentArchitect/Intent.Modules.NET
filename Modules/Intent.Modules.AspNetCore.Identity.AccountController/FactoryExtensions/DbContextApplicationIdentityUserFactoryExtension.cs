using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.ApplicationIdentityUserConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DbContextApplicationIdentityUserFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.AccountController.DbContextApplicationIdentityUserFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (application.MetadataManager.Domain(application).GetClassModels().Any(p => p.HasIdentityUser()))
            {
                return;
            }

            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            if (dbContext == null)
            {
                return;
            }

            dbContext.CSharpFile.OnBuild(file =>
            {
                var priClass = file.Classes.First();

                priClass.AddProperty($"{dbContext.UseType("Microsoft.EntityFrameworkCore.DbSet")}<{dbContext.GetTypeName("Domain.IdentityUser")}>", "ApplicationIdentityUsers");

                priClass.FindMethod("OnModelCreating")
                    .Statements.Add($"modelBuilder.ApplyConfiguration(new {dbContext.GetTypeName(ApplicationIdentityUserConfigurationTemplate.TemplateId)}());");
            });
        }
    }
}