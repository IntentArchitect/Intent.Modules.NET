using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.ModularMonolith.Module";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var domainPackage = app.GetDesigner(DomainDesignerId).GetPackages().First();
            if (domainPackage.Stereotypes.FirstOrDefault(s => s.DefinitionId == "897cc466-d518-444c-bb01-769024eee290") == null)// Database Settings            
            {
                var stereotype = new StereotypePersistable
                {
                    DefinitionId = "897cc466-d518-444c-bb01-769024eee290",
                    Name = "Database Settings",
                    DefinitionPackageId = "Intent.EntityFrameworkCore",
                    DefinitionPackageName = "a9d2a398-04e4-4300-9fbb-768568c65f9e"

                };
                stereotype.Properties =
                [
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "62bac18a-63d0-44eb-b3b8-52eb2ce6b65d",
                        Name = "Connection String Name",
                        Value = app.Name,
                    },
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "2e40f3e9-834c-4bb4-86b3-2d18e6b1266c",
                        Name = "Database Provider",
                        Value = "Default",
                    },
                ];
                domainPackage.Stereotypes.Add(stereotype);
                app.SaveAllChanges();
            }
        }
    }
}