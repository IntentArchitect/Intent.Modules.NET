using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        private const string DocumentDBProviderPropertyId = "85ea3708-f41a-4cd5-a23c-1126d9ddd4e1";
        private const string RDBMSStereotypeId = "51a7bcf5-0eb9-4c9a-855e-3ead1048729c";
        private const string IntentMetadataDocumentDBModuleId = "1c7eab18-9482-4b4e-b61b-1fbd2d2427b6";

        public static void EnsureProviderDomainPackage(IApplicationPersistable app, IMetadataInstaller metadataInstaller, ProviderContext context)
        {
            var designer = app.GetDesigner(DomainDesignerId);
            var packages = designer.GetPackages();

            var hasExistingPackage = false;
            var providerId = context.ProviderId;

            foreach (var package in packages)
            {
                if (IsRelationalPackage(package))
                {
                    continue;
                }

                // Already has a DocumentDB stereotype, that can be claimed
                if (TryEnsureProviderForExistingDocumentDb(designer, package, providerId))
                {
                    hasExistingPackage = true;
                }
                // No DocumentDB stereotype – add one for this provider
                else if (!HasDocumentDbStereotype(package))
                {
                    AddDocumentDbStereotype(designer, package, providerId);
                    hasExistingPackage = true;
                    break;
                }
            }

            if (!hasExistingPackage)
            {
                InstallNewPackage(app, metadataInstaller, context);
            }
        }

        private static bool IsRelationalPackage(IPackageModelPersistable package) =>
            package.Stereotypes.Any(x => x.DefinitionId == RDBMSStereotypeId);

        private static bool HasDocumentDbStereotype(IPackageModelPersistable package) =>
            package.Stereotypes.Any(x => x.DefinitionId == DocumentDBStereotypeId);

        private static bool TryEnsureProviderForExistingDocumentDb(IApplicationDesignerPersistable designer, IPackageModelPersistable package, string providerId)
        {
            var stereotype = package.Stereotypes
                .FirstOrDefault(x => x.DefinitionId == DocumentDBStereotypeId);

            if (stereotype == null)
            {
                return false;
            }

            var providerSetting = stereotype.Properties
                .FirstOrDefault(p => p.DefinitionId == DocumentDBProviderPropertyId);

            if (providerSetting == null)
            {
                return false;
            }

            //Already Have a package for this provider
            if (providerSetting.Value == providerId)
            {
                return true;
            }

            // Empty provider + nothing modeled, hijack this package
            var canUseThisPackage = string.IsNullOrEmpty(providerSetting.Value)
                                    && !package.Classes.Any();

            if (canUseThisPackage)
            {
                providerSetting.Value = providerId;
                designer.Save();
                return true;
            }

            return false;
        }

        private static void AddDocumentDbStereotype(IApplicationDesignerPersistable designer, IPackageModelPersistable package, string providerId)
        {
            package.Stereotypes.Add(
                DocumentDBStereotypeId,
                "Document Database",
                IntentMetadataDocumentDBModuleId,
                "Intent.Metadata.DocumentDB",
                stereotype =>
                {
                    stereotype.Properties.Add(
                        DocumentDBProviderPropertyId,
                        "Provider",
                        providerId);
                });

            designer.Save();
        }

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