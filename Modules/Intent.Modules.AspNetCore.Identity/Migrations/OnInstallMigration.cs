using Intent.Engine;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string DomainDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";

        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IPersistenceLoader persistenceLoader)
        {
            _configurationProvider = configurationProvider;
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.Identity";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadApplication(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(DomainDesignerId);
            var package = designer.GetPackages().FirstOrDefault();

            if(package is null)
            {
                return;
            }

            CreateApplicationIdentity(package);
        }

        private void CreateApplicationIdentity(IPackageModelPersistable package)
        {
            package.Classes.Add(Guid.NewGuid().ToString(), "Class", "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                "ApplicationIdentityUser", package.Id);
        }

        private void CreateAspNetIdentityExternal()
        {

        }
    }
}