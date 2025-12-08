using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.Metadata.RDBMS.Api.DomainPackageModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Migrations
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
        public string ModuleId => "Intent.EntityFrameworkCore";

        public void OnInstall()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
            DomainPackageInstallationHelper.EnsureProviderDomainPackage(app
                , _metadataInstaller
                , new ProviderContext(
                    _configurationProvider.GetApplicationConfig().Name,
                    _configurationProvider.GetSolutionConfig().SolutionName,
                    "3ad0fc2e-624e-4b61-8fad-b58231cca672", // Just a Guid for PackageId 
                    "EF"
                ));
        }
    }

    internal class DomainPackageInstallationHelper
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        private const string DocumentDBStereotypeId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        private const string RDBMSStereotypeId = "51a7bcf5-0eb9-4c9a-855e-3ead1048729c";
        private const string IntentMetadataDocumentDBModuleId = "1c7eab18-9482-4b4e-b61b-1fbd2d2427b6";
        private const string RelationalDatabaseName = "Relational Database";

        public static void EnsureProviderDomainPackage(IApplicationPersistable app, IMetadataInstaller metadataInstaller, ProviderContext context)
        {
            var designer = app.GetDesigner(DomainDesignerId);
            var packages = designer.GetPackages();

            var hasExistingPackage = false;
            var providerId = context.ProviderId;

            foreach (var package in packages)
            {
                if (IsDocumentDbPackage(package))
                {
                    continue;
                }

                hasExistingPackage = true;

                if ((package.Metadata != null && package.Metadata.Any(i => i.Key == "database-paradigm-selected")) ||
                    (package.Stereotypes != null &&
                    (package.Stereotypes.Any(s => s.DefinitionId == RDBMSStereotypeId) ||
                    package.Stereotypes.Any(s => s.DefinitionId == DocumentDBStereotypeId))))
                {
                    break;
                }

                package.Stereotypes.Add(RDBMSStereotypeId,
                    RelationalDatabaseName,
                    package.Id,
                    package.Name
                );

                package.Metadata.Add("database-paradigm-selected", "true");

                app.SaveAllChanges();

            }

            if (!hasExistingPackage)
            {
                InstallNewPackage(app, metadataInstaller, context);
            }
        }

        private static bool IsRelationalPackage(IPackageModelPersistable package) =>
            package.Stereotypes.Any(x => x.DefinitionId == RDBMSStereotypeId);

        private static bool IsDocumentDbPackage(IPackageModelPersistable package) =>
            package.Stereotypes.Any(x => x.DefinitionId == DocumentDBStereotypeId);

        private static void InstallNewPackage(IApplicationPersistable app, IMetadataInstaller metadataInstaller, ProviderContext context)
        {
            var visualStudioMetadataInstallationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../content", "domain-designer.metadata.config");
            if (File.Exists(visualStudioMetadataInstallationFile))
            {
                metadataInstaller.InstallFromFile(app.AbsolutePath, visualStudioMetadataInstallationFile,
                    fieldDictionary: new Dictionary<string, string>
                    {
                        ["application.name"] = context.AppName,
                        ["solution.name"] = context.SolutionName,
                        ["provider.id"] = context.ProviderId,
                        ["provider.name"] = context.ProviderName,
                        ["package.id"] = context.ProviderId // I need each package to have a unique ID (other wise they don't install together), but I also want it to be consistant, so reusing provider ID here
                    }, s => { return Task.CompletedTask; });
            }
            else
            {
                throw new Exception("File not found: " + visualStudioMetadataInstallationFile);
            }
        }
    }
    public record ProviderContext(string AppName, string SolutionName, string ProviderId, string ProviderName);
}