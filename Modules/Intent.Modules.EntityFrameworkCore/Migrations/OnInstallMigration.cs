using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        private const string RelationalDatabaseId = "51a7bcf5-0eb9-4c9a-855e-3ead1048729c";
        private const string RelationalDatabaseName = "Relational Database";
        private const string DocumentDatabaseId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        private readonly string[] MigrationSkipModules = ["Intent.CosmosDB", "Intent.MongoDb"];

        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IMetadataInstaller _metadataInstaller;
        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IMetadataInstaller metadataInstaller)
        {
            _configurationProvider = configurationProvider;
            _metadataInstaller = metadataInstaller;
        }

        [IntentFully]
        public string ModuleId => "Intent.EntityFrameworkCore";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(DomainDesignerId);

            var packages = designer.GetPackages();

            // if there are already multiple packages, then don't touch them.
            if (packages != null && packages.Count != 1)
            {
                return;
            }

            var package = packages.FirstOrDefault();

            // this should never happen but just incase
            if (package is null)
            {
                return;
            }

            // if the package already has a document stereotype
            if ((package.Metadata != null && package.Metadata.Any(i => i.Key == "database-paradigm-selected")) ||
                (package.Stereotypes != null &&
                (package.Stereotypes.Any(s => s.DefinitionId == RelationalDatabaseId) ||
                package.Stereotypes.Any(s => s.DefinitionId == DocumentDatabaseId))))
            {
                return;
            }

            // if any of the specific modules are installed, skip the migration.
            // this is to preserve existing functionality - if EF and Cosmo/Mongo is selected at app creation, then previously
            // the package would get the DocumentDB stereotype. This check is to keep that functionality as is.
            if (MigrationSkipModules.Any(m => app.Modules.Any(mod => mod.ModuleId == m)))
            {
                return;
            }

            package.Stereotypes ??= [];
            package.Stereotypes.Add(new IArchitect.Agent.Persistence.Model.Common.StereotypePersistable
            {
                DefinitionId = RelationalDatabaseId,
                Name = RelationalDatabaseName,
                DefinitionPackageId = package.Id,
                DefinitionPackageName = package.Name
            });

            package.Metadata ??= [];
            package.Metadata.Add(new GenericMetadataPersistable
            {
                Key = "database-paradigm-selected",
                Value = "true"
            });

            app.SaveAllChanges();
        }
    }
}