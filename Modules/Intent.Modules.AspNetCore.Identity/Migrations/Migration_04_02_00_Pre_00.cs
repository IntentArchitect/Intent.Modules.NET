using System;
using System.Linq;
using Intent.Engine;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.Migrations
{
    public class Migration_04_02_00_Pre_00 : IModuleMigration
    {
        private const string DomainDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";

        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_04_02_00_Pre_00(IApplicationConfigurationProvider configurationProvider, IPersistenceLoader persistenceLoader)
        {
            _configurationProvider = configurationProvider;
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.Identity";
        [IntentFully]
        public string ModuleVersion => "4.2.0-pre.0";

        public void Up()
        {
            var app = _persistenceLoader.LoadApplication(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(DomainDesignerId);
            var package = designer.GetPackages(true).FirstOrDefault();
            if (package is null)
            {
                return;
            }

            CreateApplicationIdentity(package);
        }

        private void CreateApplicationIdentity(IPackageModelPersistable package)
        {
            var identityApplicationUserEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationUser", identityApplicationUserEntityId);

            var identityApplicationRoleEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationRole", identityApplicationRoleEntityId);

            var identityApplicationUserClaimEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationUserClaim", identityApplicationUserClaimEntityId);

            var identityApplicationUserRoleEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationUserRole", identityApplicationUserRoleEntityId);

            var identityApplicationUserLoginEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationUserLogin", identityApplicationUserLoginEntityId);

            var identityApplicationUserRoleClaimEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationRoleClaim", identityApplicationUserRoleClaimEntityId);

            var identityApplicationUserTokenEntityId = CreateAspNetIdentityExternal(package, "IdentityUser");
            CreateEntity(package, "ApplicationUserToken", identityApplicationUserTokenEntityId);
        }

        private void CreateEntity(IPackageModelPersistable package, string entityName, string inheritFromId)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = package.Classes.Add(entityId, "Class", "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                entityName, package.Id);

            entity.ExternalReference = inheritFromId;
        }

        private string CreateAspNetIdentityExternal(IPackageModelPersistable externalPackage, string entityName)
        {
            var entityId = Guid.NewGuid().ToString();

            var entity = externalPackage.Classes.Add(entityId, "Class", "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                entityName, externalPackage.Id);

            return entity.Id;
        }

        public void Down()
        {
        }
    }
}