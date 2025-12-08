using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Metadata;
using Intent.Modules.Azure.TableStorage.Templates;
using Intent.Modules.DocumentDB.Shared;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {

        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IMetadataInstaller _metadataInstaller;
        private readonly IPersistenceLoader _persistenceLoader;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IMetadataInstaller metadataInstaller, IPersistenceLoader persistenceLoader)
        {
            _configurationProvider = configurationProvider;
            _metadataInstaller = metadataInstaller;
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Azure.TableStorage";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            DomainPackageInstallationHelper.EnsureProviderDomainPackage(app
                , _metadataInstaller
                , new ProviderContext(
                    _configurationProvider.GetApplicationConfig().Name,
                    _configurationProvider.GetSolutionConfig().SolutionName,
                    TableStorageProvider.Id,
                    "TableStorage"
                ));
        }
    }
}