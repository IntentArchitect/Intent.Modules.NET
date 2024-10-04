using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata;
using Intent.Plugins;

namespace Intent.Modules.Blazor.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly IMetadataInstaller _metadataInstaller;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider, IMetadataInstaller metadataInstaller)
        {
            _configurationProvider = configurationProvider;
            _metadataInstaller = metadataInstaller;
        }

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(RoleSpecializationId);
                if (elements.Any(x => x.Name == "Blazor"))
                {
                    return;
                }
            }

            var visualStudioMetadataInstallationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../content", "visual-studio.metadata.config");
            if (File.Exists(visualStudioMetadataInstallationFile))
            {
                _metadataInstaller.InstallFromFile(app.AbsolutePath, visualStudioMetadataInstallationFile,
                    fieldDictionary: new Dictionary<string, string>
                    {
                        ["application.name"] = _configurationProvider.GetApplicationConfig().Name,
                        ["solution.name"] = _configurationProvider.GetSolutionConfig().SolutionName
                    }, s => { return Task.CompletedTask; });
            }
            else
            {
                throw new Exception("File not found: " + visualStudioMetadataInstallationFile);
            }
        }

        public void OnUninstall()
        {
            // TODO: Should uninstall client?
            //var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            //var designer = app.GetDesigner(VisualStudioDesignerId);
            //var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            //foreach (var package in packages)
            //{
            //    var elements = package.GetElementsOfType(RoleSpecializationId);
            //    if (elements.Any(x => x.Name == "Blazor"))
            //    {
            //        return;
            //    }
            //}
        }

        public string ModuleId => "Intent.Blazor";
    }
}
