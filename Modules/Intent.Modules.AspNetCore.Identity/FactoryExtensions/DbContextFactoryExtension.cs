using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class DbContextFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.DbContextFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var dbContextTemplate = application.FindTemplateInstance<DbContextTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DbContext"));
            if (dbContextTemplate == null)
            {
                return;
            }

            dbContextTemplate.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(dbContextTemplate.OutputTarget.GetProject()));

            dbContextTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                @class.WithBaseType($"{dbContextTemplate.UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{dbContextTemplate.GetIdentityUserClass()}>");
            });
        }
    }
    
    public static class IdentityHelperExtensions
    {
        public static string GetIdentityUserClass<T>(this CSharpTemplateBase<T> template)
        {
            var identityModel = template.ExecutionContext.MetadataManager.Domain(template.ExecutionContext.GetApplicationConfig().Id).GetClassModels()
                .SingleOrDefault(x => x.HasIdentityUser());
            var identityUserClass = identityModel != null
                ? template.GetTypeName("Domain.Entity", identityModel)
                : template.TryGetTypeName("Domain.IdentityUser");

            return identityUserClass ?? template.UseType("Microsoft.AspNetCore.Identity.IdentityUser");
        }

        public static string GetIdentityRoleClass<T>(this CSharpTemplateBase<T> template)
        {
            return template.UseType("Microsoft.AspNetCore.Identity.IdentityRole");
        }
    }
}