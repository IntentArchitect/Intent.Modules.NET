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
            foreach (var @class in application.MetadataManager.Domain(application).GetClassModels())
            {
                if(@class.ParentClass is not null)
                {
                    UpdateEntityConfiguration(application, @class.ParentClass.Name switch
                    {
                        "IdentityUser" or "IdentityRole" or "IdentityUserRole" or
                            "IdentityRoleClaim" or "IdentityUserClaim" or "IdentityUserToken" or "IdentityUserLogin" => @class,
                        _ => null
                    });
                    switch (@class.ParentClass.Name)
                    {
                        case "IdentityUser":

                        default:
                            break;
                    }
                }
            }
        }

        private void UpdateEntityConfiguration(IApplication application, ClassModel? userIdentityEntity)
        {
            if(userIdentityEntity is null || userIdentityEntity.ParentClass is null)
            {
                return;
            }

            var entityTypeConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.EntityTypeConfiguration", userIdentityEntity.Id);
            if (entityTypeConfigTemplate is null)
            {
                return;
            }

            entityTypeConfigTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var configMethod = @class.FindMethod("Configure");
                configMethod.FindAndReplaceStatement(s => s.Text.Contains("builder.HasBaseType"), new Common.CSharp.Builder.CSharpStatement($"builder.HasBaseType<{userIdentityEntity.ParentClass.Name}<string>>();"));
            });
        }
    }
}