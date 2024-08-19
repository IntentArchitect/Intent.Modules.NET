using System;
using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

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

#warning is this actually used and just lying around here - 16 Aug 2024
    [IntentManaged(Mode.Ignore)]
    public class DbContextFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.DbContextFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var identityModel = application.MetadataManager.GetIdentityUserClass(application.GetApplicationConfig().Id);

            UpdateDbContext(application, identityModel);
            UpdateEntityTemplate(application, identityModel);
        }

        private static void UpdateDbContext(IApplication application, ClassModel identityModel)
        {
            var dbContextTemplate =
                application.FindTemplateInstance<DbContextTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DbContext"));
            if (dbContextTemplate == null)
            {
                return;
            }
            dbContextTemplate.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(dbContextTemplate.OutputTarget.GetProject()));

            dbContextTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                @class.WithBaseType($"{dbContextTemplate.UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{dbContextTemplate.GetIdentityUserClass()}>");

                if (identityModel != null && (
                        identityModel.Name.Equals("User", StringComparison.OrdinalIgnoreCase) ||
                        identityModel.Name.Equals("User", StringComparison.OrdinalIgnoreCase))
                    )
                {
                    var usersProperty = @class.Properties.SingleOrDefault(x => x.Name == "Users");
                    if (usersProperty != null)
                    {
                        @class.Properties.Remove(usersProperty);
                    }
                }
            });
        }

        private static void UpdateEntityTemplate(IApplication application, ClassModel identityModel)
        {
            if (identityModel == null)
            {
                return;
            }

            var idAttribute = identityModel.Attributes.SingleOrDefault(x => x.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
            var textConstraints = idAttribute?.GetTextConstraints();

            if (identityModel.ParentClass != null ||
                idAttribute == null ||
                !idAttribute.HasPrimaryKey() ||
                idAttribute.TypeReference?.Element?.Name != "string" ||
                textConstraints?.SQLDataType().IsDEFAULT() != true ||
                textConstraints.MaxLength() != 450)
            {
                Logging.Log.Failure($"When the \"Identity User\" stereotype is applied to a class, it must have an attribute with all the following characteristics:{Environment.NewLine}" +
                                    $"- It cannot be inheriting from another class{Environment.NewLine}" +
                                    $"- A \"Primary Key\" stereotype applied{Environment.NewLine}" +
                                    $"- A name of \"id\"{Environment.NewLine}" +
                                    $"- Its type set to \"string\"{Environment.NewLine}" +
                                    $"- The \"Text Constraints\" stereotype applied to it{Environment.NewLine}" +
                                    $"- Its \"Text Constraints\" stereotype's \"SQL Data Type\" property must be set to \"DEFAULT\"{Environment.NewLine}" +
                                    $"- Its \"Text Constraints\" stereotype's \"MaxLength\" property must be set to \"450\"{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"Update the \"{identityModel.Name}\" [{identityModel.Id}] class in the Domain Designer to ensure it meets the above requirements or " +
                                    $"remove the \"Identity User\" Stereotype from the class.");
                return;
            }


            var entityTemplate = application
                .FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnModel(TemplateRoles.Domain.Entity.Primary, identityModel));
            if (entityTemplate == null)
            {
                return;
            }

            entityTemplate.FulfillsRole("Domain.IdentityUser");
            entityTemplate.AddNugetDependency(NugetPackages.MicrosoftExtensionsIdentityStores(entityTemplate.OutputTarget.GetProject()));
            entityTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.Identity");
                var @class = file.Classes.First();
                @class.ExtendsClass(entityTemplate.UseType("Microsoft.AspNetCore.Identity.IdentityUser"));

                var property = @class.Properties.SingleOrDefault(x => x.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    @class.Properties.Remove(property);
                }
            });
        }
    }
}