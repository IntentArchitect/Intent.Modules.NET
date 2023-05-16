using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityUserFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.IdentityUserFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;
    }
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
            if (dbContextTemplate != null)
            {
                dbContextTemplate.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(dbContextTemplate.OutputTarget.GetProject()));

                dbContextTemplate.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.WithBaseType($"{dbContextTemplate.UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{dbContextTemplate.GetIdentityUserClass()}>");
                });
            }

            var identityModel = application.MetadataManager.Domain(application.GetApplicationConfig().Id).GetClassModels()
                .SingleOrDefault(x => x.HasIdentityUser());
            if (identityModel == null)
            {
                return;
            }
            var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnModel(TemplateFulfillingRoles.Domain.Entity.Primary, identityModel));
            if (entityTemplate != null)
            {
                entityTemplate.AddNugetDependency(NugetPackages.MicrosoftExtensionsIdentityStores(entityTemplate.OutputTarget.GetProject()));
                entityTemplate.CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("Microsoft.AspNetCore.Identity");
                    var @class = file.Classes.First();
                    @class.ExtendsClass(entityTemplate.UseType("Microsoft.AspNetCore.Identity.IdentityUser"));
                });
            }
        }
    }
}