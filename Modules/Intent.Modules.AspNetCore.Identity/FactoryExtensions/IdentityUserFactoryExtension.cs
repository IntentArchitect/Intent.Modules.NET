using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityUserFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.IdentityUserFactoryExtension";

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

            if (identityModel.ParentClass?.Name != "IdentityUser")
            {
                Logging.Log.Failure($"When the \"Identity User\" stereotype is applied to a class it must derive from the IdentityUser class. " +
                                    $"Update the \"{identityModel.Name}\" [{identityModel.Id}] class in the Domain Designer to ensure it meets " +
                                    $"this requirements or remove the \"Identity User\" Stereotype from the class.");
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

                if (identityModel.ParentClass.IsIdentityUserBaseClass())
                {
                    if (!@class.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks))
                    {
                        pks = [];
                    }

                    var baseClassPks = identityModel.ParentClass.Attributes
                        .Where(x => x.HasPrimaryKey())
                        .Select(x =>
                        {
                            var property = new CSharpProperty(x.TypeReference.Element?.Name, x.Name, null);
                            property.AddMetadata("model", x);
                            return property;
                        });

                    @class.Metadata["primary-keys"] = baseClassPks.Concat(pks).ToArray();
                }
                else
                {
                    @class.ExtendsClass(entityTemplate.UseType("Microsoft.AspNetCore.Identity.IdentityUser"));

                    var property = @class.Properties.SingleOrDefault(x => x.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        @class.Properties.Remove(property);
                    }
                }
            }, -10);
        }
    }
}