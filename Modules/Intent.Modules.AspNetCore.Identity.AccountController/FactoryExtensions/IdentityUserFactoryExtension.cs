using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityUserFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.AccountController.IdentityUserFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 100;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var userIdentityEntity = application.MetadataManager.Domain(application).GetClassModels().FirstOrDefault(p => p.HasIdentityUser());
            if (userIdentityEntity is null)
            {
                return;
            }

            UpdateDbContext(application, userIdentityEntity);
            UpdateEntityTemplate(application, userIdentityEntity);
        }

        private void UpdateDbContext(IApplication application, ClassModel userIdentityEntity)
        {
            var entityTypeConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.EntityTypeConfiguration", userIdentityEntity.Id);
            if (entityTypeConfigTemplate is null)
            {
                return;
            }

            entityTypeConfigTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var configMethod = @class.FindMethod("Configure");
                configMethod.AddStatement($"builder.Property(x => x.RefreshToken);");
                configMethod.AddStatement($"builder.Property(x => x.RefreshTokenExpired);");
            });
        }

        private void UpdateEntityTemplate(IApplication application, ClassModel userIdentityEntity)
        {
            var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Domain.Entity", userIdentityEntity.Id);
            if (entityTemplate is null)
            {
                return;
            }

            entityTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.AddProperty(entityTemplate.UseType("string?"), "RefreshToken");
                @class.AddProperty(entityTemplate.UseType("System.DateTime?"), "RefreshTokenExpired");
            });
        }
    }
}