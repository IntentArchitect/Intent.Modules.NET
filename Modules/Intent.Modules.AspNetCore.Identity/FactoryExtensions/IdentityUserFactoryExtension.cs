using System;
using System.Collections.Frozen;
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

        private static DbContextTemplate? GetDbContextTemplate(IApplication application)
        {
            var result = application.FindTemplateInstance<DbContextTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DbContext"));
            if (result != null) return result;
            result = application.FindTemplateInstance<DbContextTemplate>(TemplateDependency.OnTemplate("Intent.EntityFrameworkCore.DbContext"));
            return result;
        }

        private static void UpdateDbContext(IApplication application, ClassModel identityModel)
        {
            var dbContextTemplate = GetDbContextTemplate(application);
            if (dbContextTemplate == null)
            {
                return;
            }
            dbContextTemplate.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(dbContextTemplate.OutputTarget.GetProject()));

            dbContextTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                file.AddUsing("Microsoft.AspNetCore.Identity");
                @class.WithBaseType($"{dbContextTemplate.UseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext")}<{dbContextTemplate.GetIdentityUserClasses()}>");

                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityUser>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityUser>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityRole>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityRole>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityUserLogin>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityUserLogin>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityUserToken>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityUserToken>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityUserRole>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityUserRole>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityRoleClaim>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityRoleClaim>"));
                }
                if (@class.Properties.Any(p => p.Type == "DbSet<IdentityUserClaim>"))
                {
                    @class.Properties.Remove(@class.Properties.First(p => p.Type == "DbSet<IdentityUserClaim>"));
                }

                if (@class.Methods.Any(m => m.Name == "OnModelCreating"))
                {
                    var method = @class.Methods.First(m => m.Name == "OnModelCreating");
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserRoleConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserRoleConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserLoginConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserLoginConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserTokenConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserTokenConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityRoleClaimConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityRoleClaimConfiguration());"));
                    }
                    if (method.Statements.Any(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserClaimConfiguration());"))
                    {
                        method.Statements.Remove(method.Statements.First(s => s.Text == "modelBuilder.ApplyConfiguration(new IdentityUserClaimConfiguration());"));
                    }
                }

                // Users Exists on the base class already
                if (identityModel != null && identityModel.Name.Equals("User", StringComparison.OrdinalIgnoreCase))
                {

                    var usersProperty = @class.Properties.SingleOrDefault(x => x.Name == "Users");
                    if (usersProperty != null)
                    {
                        @class.Properties.Remove(usersProperty);
                    }
                }
            }, 1);
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