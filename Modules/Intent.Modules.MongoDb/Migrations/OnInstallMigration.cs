using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata;
using Intent.Metadata.Models;
using Intent.Modules.DocumentDB.Shared;
using Intent.Modules.MongoDb.Templates;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.MongoDb.Migrations
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
        public string ModuleId => "Intent.MongoDb";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            DomainPackageInstallationHelper.EnsureProviderDomainPackage(app
                , _metadataInstaller
                , new ProviderContext(
                    _configurationProvider.GetApplicationConfig().Name,
                    _configurationProvider.GetSolutionConfig().SolutionName,
                    MongoDbProvider.Id,
                    "MongoDB"
                ));
        }
    }
}